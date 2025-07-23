using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormsController : ControllerBase
    {
        private readonly FormRepository _formRepository;

        public FormsController(FormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm([FromBody] CreateFormViewModel model)
        {
            try
            {
                var formId = await _formRepository.CreateFormAsync(model);
                return Ok(new { success = true, formId = formId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllForms()
        {
            var forms = await _formRepository.GetAllFormsAsync();
            return Ok(forms);
        }
    }
}
