using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using DidUFall4It_DDACGroupAssignment_Group21.Data;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class InfographicController : Controller
    {
        private readonly DidUFall4It_DDACGroupAssignment_Group21Context _context;
        private readonly IWebHostEnvironment _environment;

        public InfographicController(IWebHostEnvironment environment, DidUFall4It_DDACGroupAssignment_Group21Context context)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult InfoHome()
        {
            return View();
        }
        public IActionResult InsightsHome()
        {
            return View();
        }

        public IActionResult InfoCreate()
        {
            return View();
        }

        public IActionResult InfoList()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InfoCreate(InfographicModel model, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    model.ImagePath = "/uploads/" + uniqueFileName;
                }

                _context.Infographics.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("InfoList", "Infographic");
            }

            return View(model);
        }
    }
}
