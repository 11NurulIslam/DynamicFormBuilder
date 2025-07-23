namespace DynamicFormBuilder.Models
{
    public class FormField
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public string Label { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public string SelectedOption { get; set; } = string.Empty;
        public int FieldOrder { get; set; }
    }
}
