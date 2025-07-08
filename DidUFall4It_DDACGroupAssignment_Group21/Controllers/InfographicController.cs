using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
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
        private List<string> getValues()
        {
            List<string> values = new List<string>();
            //1. link to appsettings.json and get back the values
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build(); //build the json file
                                                            //2. read the info from json using configure instance
            values.Add(configure["Values:Key1"]);
            values.Add(configure["Values:Key2"]);
            values.Add(configure["Values:Key3"]);
            return values;
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

        [HttpPost]
        public IActionResult DeleteInsights()
        {
            ViewBag.MetricAverage = null;
            ViewBag.SelectedMetric = null;
            ViewBag.RecentComments = null;

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
                    if (ImageFile.Length <= 0)
                    {
                        return BadRequest("It is an empty file. Unable to upload!");
                    }
                    else if (ImageFile.ContentType.ToLower() != "image/png" && ImageFile.ContentType.ToLower() != "image/jpeg"
                    && ImageFile.ContentType.ToLower() != "image/gif")
                    {
                        return BadRequest("It is not a valid image! Unable to upload!");
                    }

                    try
                    {
                        // Ensure the uploads folder exists inside wwwroot
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Generate unique file name and save it
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        // Save relative URL to model
                        model.ImagePath = "/uploads/" + uniqueFileName;
                        model.ImageKey = uniqueFileName; // You can use this later if needed
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Unable to upload due to technical issue. Error message: " + ex.Message);
                    }
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
                    var existing = await _context.Infographics.FindAsync(id);
                    if (existing == null) return NotFound();

                    // Update text fields
                    existing.Title = model.Title;
                    existing.Description = model.Description;
                    existing.Tips = model.Tips;

                    // Handle new image upload
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(existing.ImageKey))
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", existing.ImageKey);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // Save new image
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                        string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        string savePath = Path.Combine(uploadFolder, uniqueFileName);

                        // Ensure uploads folder exists
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        // Update model paths
                        existing.ImagePath = "/uploads/" + uniqueFileName;
                        existing.ImageKey = uniqueFileName;
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

            // Delete the image using ImageKey instead of ImagePath
            if (!string.IsNullOrEmpty(infographic.ImageKey))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", infographic.ImageKey);
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
