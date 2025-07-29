using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Hosting;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
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
                        return BadRequest("It is an empty file. Unable to upload!");

                    if (ImageFile.Length > 1048576)
                        return BadRequest("The file is too large. Maximum allowed size is 1MB.");

                    if (ImageFile.ContentType.ToLower() != "image/png" &&
                        ImageFile.ContentType.ToLower() != "image/jpeg" &&
                        ImageFile.ContentType.ToLower() != "image/gif")
                    {
                        return BadRequest("It is not a valid image! Unable to upload!");
                    }

                    try
                    {
                        // Get AWS credentials and bucket name from appsettings
                        List<string> values = getValues();
                        string accessKey = values[0];
                        string secretKey = values[1];
                        string bucketName = values[2];

                        var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast1); // Use appropriate region

                        // Generate a unique filename for S3
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);

                        // Upload to S3 using TransferUtility
                        using (var newMemoryStream = new MemoryStream())
                        {
                            await ImageFile.CopyToAsync(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = newMemoryStream,
                                Key = uniqueFileName,
                                BucketName = bucketName,
                                ContentType = ImageFile.ContentType,
                                CannedACL = S3CannedACL.PublicRead // Make it public if needed
                            };

                            var transferUtility = new TransferUtility(s3Client);
                            await transferUtility.UploadAsync(uploadRequest);
                        }

                        // Save S3 URL or key
                        model.ImagePath = $"https://{bucketName}.s3.amazonaws.com/{uniqueFileName}";
                        model.ImageKey = uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Unable to upload to S3. Error: " + ex.Message);
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

                    if (ImageFile != null)
                    {
                        if (ImageFile.Length <= 0)
                        {
                            ModelState.AddModelError("ImageFile", "It is an empty file. Unable to upload!");
                            return View(model);
                        }
                        else if (ImageFile.Length > 1048576) // 1MB
                        {
                            ModelState.AddModelError("ImageFile", "The file is too large. Maximum allowed size is 1MB.");
                            return View(model);
                        }
                        else if (ImageFile.ContentType.ToLower() != "image/png"
                              && ImageFile.ContentType.ToLower() != "image/jpeg"
                              && ImageFile.ContentType.ToLower() != "image/gif")
                        {
                            ModelState.AddModelError("ImageFile", "Invalid image format. Only PNG, JPEG, and GIF are allowed.");
                            return View(model);
                        }

                        // Load AWS credentials and bucket
                        var values = getValues();
                        var accessKey = values[0];
                        var secretKey = values[1];
                        var bucketName = values[2];

                        var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.APSoutheast1);

                        // Delete existing image from S3 if exists
                        if (!string.IsNullOrEmpty(existing.ImageKey))
                        {
                            try
                            {
                                await s3Client.DeleteObjectAsync(bucketName, existing.ImageKey);
                            }
                            catch (Exception ex)
                            {
                                // Optional: log this error if deletion fails
                            }
                        }

                        // Generate new unique filename
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);

                        // Upload new image to S3
                        using (var newMemoryStream = new MemoryStream())
                        {
                            await ImageFile.CopyToAsync(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = newMemoryStream,
                                Key = uniqueFileName,
                                BucketName = bucketName,
                                ContentType = ImageFile.ContentType,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(s3Client);
                            await fileTransferUtility.UploadAsync(uploadRequest);
                        }

                        // Update model with new image info
                        existing.ImagePath = $"https://{bucketName}.s3.amazonaws.com/{uniqueFileName}";
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
