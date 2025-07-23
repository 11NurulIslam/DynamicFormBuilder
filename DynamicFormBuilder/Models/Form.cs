using Microsoft.AspNetCore.Http;

namespace DynamicFormBuilder.Models
{
    public class Form
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public List<FormField> Fields { get; set; } = new List<FormField>();
    }
}
