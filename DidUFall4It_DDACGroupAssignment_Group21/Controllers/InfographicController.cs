using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class InfographicController : Controller
    {
        private readonly DidUFall4It_DDACGroupAssignment_Group21Context _context;
        private readonly IWebHostEnvironment _environment;
        private string bucketNameS3 = "didyoufall4it-bucket";
        private string snsTopicArn = "arn:aws:sns:us-east-1:859425880780:success-image-upload";
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
                //if (ImageFile != null)
                //{
                //    try
                //    {

                //        List<string> values = getValues();

                //        var s3Client = new AmazonS3Client(values[0], values[1], values[2], RegionEndpoint.USEast1);
                //        var snsClient = new AmazonSimpleNotificationServiceClient(values[0], values[1], values[2], RegionEndpoint.USEast1);


                //        string uniqueFileName = "infographics/" + Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);

                //        using (var newMemoryStream = new MemoryStream())
                //        {
                //            await ImageFile.CopyToAsync(newMemoryStream);

                //            PutObjectRequest uploadRequest = new PutObjectRequest
                //            {
                //                InputStream = newMemoryStream,
                //                Key = uniqueFileName,
                //                BucketName = bucketNameS3,
                //                ContentType = ImageFile.ContentType,
                //                CannedACL = S3CannedACL.PublicRead 
                //            };

                //            await s3Client.PutObjectAsync(uploadRequest);
                //        }
                //        model.ImageKey = uniqueFileName;
                //        model.ImagePath = $"https://{bucketNameS3}.s3.amazonaws.com/{uniqueFileName}";
                //        var snsRequest = new PublishRequest
                //        {
                //            TopicArn = snsTopicArn, // ← you need to set this variable
                //            Subject = "Infographic Uploaded",
                //            Message = $"An infographic has been uploaded: {model.ImagePath}"
                //        };
                //        await snsClient.PublishAsync(snsRequest);
                //    }
                //    catch (Exception ex)
                //    {
                //        return BadRequest("Error uploading to S3: " + ex.Message);
                //    }
                //}

                if (ImageFile != null)
                {
                    using (var client = new HttpClient())
                    using (var content = new MultipartFormDataContent())
                    using (var stream = new MemoryStream())
                    {
                        await ImageFile.CopyToAsync(stream);
                        stream.Position = 0;

                        content.Add(new StreamContent(stream), "file", ImageFile.FileName);

                        var response = await client.PostAsync("https://your-api-gateway-url.com/upload", content);
                        if (!response.IsSuccessStatusCode)
                            return BadRequest("Upload failed: " + await response.Content.ReadAsStringAsync());

                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(responseContent);

                        model.ImageKey = data.imageKey;
                        model.ImagePath = data.imageUrl;
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
                            ModelState.AddModelError("ImageFile", "Empty file. Unable to upload!");
                            return View(model);
                        }

                        List<string> values = getValues();

                        var s3Client = new AmazonS3Client(values[0], values[1], values[2], RegionEndpoint.USEast1);
                        var snsClient = new AmazonSimpleNotificationServiceClient(values[0], values[1], values[2], RegionEndpoint.USEast1);

                        // Delete previous image if exists
                        if (!string.IsNullOrEmpty(existing.ImageKey))
                        {
                            try
                            {
                                await s3Client.DeleteObjectAsync(bucketNameS3, existing.ImageKey);
                            }
                            catch { /* optional logging */ }
                        }

                        string uniqueFileName = "infographics/" + Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);

                        using (var newMemoryStream = new MemoryStream())
                        {
                            await ImageFile.CopyToAsync(newMemoryStream);

                            PutObjectRequest uploadRequest = new PutObjectRequest
                            {
                                InputStream = newMemoryStream,
                                Key = uniqueFileName,
                                BucketName = bucketNameS3,
                                ContentType = ImageFile.ContentType,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            await s3Client.PutObjectAsync(uploadRequest);
                        }
                        var snsRequest = new PublishRequest
                        {
                            TopicArn = snsTopicArn, 
                            Subject = "Infographic Uploaded",
                            Message = $"An infographic has been uploaded: {model.ImagePath}"
                        };
                        await snsClient.PublishAsync(snsRequest);

                        // Update DB with new S3 image key + path
                        existing.ImageKey = uniqueFileName;
                        existing.ImagePath = $"https://{bucketNameS3}.s3.amazonaws.com/{uniqueFileName}";
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
