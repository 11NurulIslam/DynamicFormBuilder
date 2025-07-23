namespace DynamicFormBuilder.Models.ViewModels
{
    public class CreateFormViewModel
    {
        public string Title { get; set; } = string.Empty;
        public List<FormFieldViewModel> Fields { get; set; } = new List<FormFieldViewModel>();
    }
}
