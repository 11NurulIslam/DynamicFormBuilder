using DynamicFormBuilder.Models;
using DynamicFormBuilder.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DynamicFormBuilder.Data
{
    public class FormRepository
    {
        private readonly string _connectionString;

        public FormRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<int> CreateFormAsync(CreateFormViewModel model)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // Insert Form
                var formQuery = "INSERT INTO Forms (Title) OUTPUT INSERTED.Id VALUES (@Title)";
                using var formCommand = new SqlCommand(formQuery, connection, transaction);
                formCommand.Parameters.AddWithValue("@Title", model.Title);

                int formId = (int)await formCommand.ExecuteScalarAsync();

                // Insert Form Fields
                for (int i = 0; i < model.Fields.Count; i++)
                {
                    var field = model.Fields[i];
                    var fieldQuery = @"INSERT INTO FormFields (FormId, Label, IsRequired, SelectedOption, FieldOrder) 
                                     VALUES (@FormId, @Label, @IsRequired, @SelectedOption, @FieldOrder)";

                    using var fieldCommand = new SqlCommand(fieldQuery, connection, transaction);
                    fieldCommand.Parameters.AddWithValue("@FormId", formId);
                    fieldCommand.Parameters.AddWithValue("@Label", field.Label);
                    fieldCommand.Parameters.AddWithValue("@IsRequired", field.IsRequired);
                    fieldCommand.Parameters.AddWithValue("@SelectedOption", field.SelectedOption ?? "");
                    fieldCommand.Parameters.AddWithValue("@FieldOrder", i + 1);

                    await fieldCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return formId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Form>> GetAllFormsAsync()
        {
            var forms = new List<Form>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT Id, Title, CreatedDate FROM Forms ORDER BY CreatedDate DESC";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                forms.Add(new Form
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    CreatedDate = reader.GetDateTime("CreatedDate")
                });
            }

            return forms;
        }

        public async Task<Form?> GetFormByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Get Form
            var formQuery = "SELECT Id, Title, CreatedDate FROM Forms WHERE Id = @Id";
            using var formCommand = new SqlCommand(formQuery, connection);
            formCommand.Parameters.AddWithValue("@Id", id);

            Form? form = null;
            using var formReader = await formCommand.ExecuteReaderAsync();
            if (await formReader.ReadAsync())
            {
                form = new Form
                {
                    Id = formReader.GetInt32("Id"),
                    Title = formReader.GetString("Title"),
                    CreatedDate = formReader.GetDateTime("CreatedDate")
                };
            }
            formReader.Close();

            if (form == null) return null;

            // Get Form Fields
            var fieldsQuery = @"SELECT Id, Label, IsRequired, SelectedOption, FieldOrder 
                              FROM FormFields WHERE FormId = @FormId ORDER BY FieldOrder";
            using var fieldsCommand = new SqlCommand(fieldsQuery, connection);
            fieldsCommand.Parameters.AddWithValue("@FormId", id);

            using var fieldsReader = await fieldsCommand.ExecuteReaderAsync();
            while (await fieldsReader.ReadAsync())
            {
                form.Fields.Add(new FormField
                {
                    Id = fieldsReader.GetInt32("Id"),
                    FormId = id,
                    Label = fieldsReader.GetString("Label"),
                    IsRequired = fieldsReader.GetBoolean("IsRequired"),
                    SelectedOption = fieldsReader.GetString("SelectedOption"),
                    FieldOrder = fieldsReader.GetInt32("FieldOrder")
                });
            }

            return form;
        }
    }
}
