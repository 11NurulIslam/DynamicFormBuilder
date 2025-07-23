using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DynamicFormBuilder.Controllers
{
    public class HomeController : Controller
    {
        private readonly FormRepository _formRepository;

        public HomeController(FormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task<IActionResult> Index()
        {
            var forms = await _formRepository.GetAllFormsAsync();
            return View(forms);
        }

        public IActionResult Create()
        {
            return View(new CreateFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _formRepository.CreateFormAsync(model);
                TempData["Success"] = "Form created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the form.");
                return View(model);
            }
        }

        public async Task<IActionResult> Preview(int id)
        {
            var form = await _formRepository.GetFormByIdAsync(id);
            if (form == null)
            {
                return NotFound();
            }
            return View(form);
        }

    }
}
