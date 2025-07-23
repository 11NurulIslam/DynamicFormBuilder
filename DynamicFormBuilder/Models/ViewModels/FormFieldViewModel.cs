namespace DynamicFormBuilder.Models.ViewModels
{
    public class FormFieldViewModel
    {
        public string Label { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public string SelectedOption { get; set; } = string.Empty;
    }
}
