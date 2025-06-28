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

        public IActionResult InfographicFeedbacks()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateInsights(string selectedMetric)
        {
            if (string.IsNullOrEmpty(selectedMetric)) return RedirectToAction("Insights");

            var feedbacks = await _context.InfographicFeedback.ToListAsync();

            double average = selectedMetric switch
            {
                "InformativeRating" => feedbacks.Average(f => f.InformativeRating),
                "EngagementRating" => feedbacks.Average(f => f.EngagementRating),
                "ClarityRating" => feedbacks.Average(f => f.ClarityRating),
                "RelevanceRating" => feedbacks.Average(f => f.RelevanceRating),
                _ => 0
            };

            var recentComments = feedbacks
                .Where(f => !string.IsNullOrWhiteSpace(f.Comment))
                .OrderByDescending(f => f.PostedAt)
                .Take(5)
                .ToList();

            ViewBag.MetricAverage = average.ToString("0.00");
            ViewBag.SelectedMetric = selectedMetric.Replace("Rating", "");
            ViewBag.RecentComments = recentComments;

            return View("InfographicFeedbacks");
        }

        public IActionResult InfoCreate()
        {
            return View();
        }

        public async Task<IActionResult> InfoList()
        {
            var infographics = await _context.Infographics
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

            var model = new InfographicViewModel(infographics);
            return View(model);
        }
        public async Task<IActionResult> InfoEdit(int id)
        {
            var infographic = await _context.Infographics.FindAsync(id);
            if (infographic == null)
            {
                return NotFound();
            }
            return View(infographic);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InfoEdit(int id, InfographicModel model, IFormFile? ImageFile)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update fields
                    var existing = await _context.Infographics.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.Title = model.Title;
                    existing.Description = model.Description;
                    existing.Tips = model.Tips;

                    // Optional: handle new image
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(ImageFile.FileName);
                        var savePath = Path.Combine("wwwroot/uploads", fileName);

                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        existing.ImagePath = "/uploads/" + fileName;
                    }

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("InfoList");
                }
                catch
                {
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InfoDeleteConfirmed(int id)
        {
            var infographic = await _context.Infographics.FindAsync(id);
            if (infographic == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(infographic.ImagePath))
            {
                var imagePath = Path.Combine("wwwroot", infographic.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Infographics.Remove(infographic);
            await _context.SaveChangesAsync();
            return RedirectToAction("InfoList");
        }

        public async Task<IActionResult> InfoGetList()
        {
            var list = await _context.Infographics
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return View(list);
        }
    }
}
