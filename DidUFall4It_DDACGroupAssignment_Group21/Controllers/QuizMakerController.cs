using Amazon; //for linking your AWS account
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; //appsettings.json section
using System.IO; // input output
using System.Text;
using System.Text.Json;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class QuizMakerController : Controller
    {
        private readonly DidUFall4It_DDACGroupAssignment_Group21Context _context;
        private readonly IWebHostEnvironment _environment;
        private const string BucketName = "didyoufall4it-infographic-bucket"; 

        public QuizMakerController(IWebHostEnvironment environment, DidUFall4It_DDACGroupAssignment_Group21Context context)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult QuizHome()
        {
            return View();
        }

        public IActionResult QuizCreate()
        {
            return View();
        }
        public IActionResult QuestionCreate()
        {
            ViewBag.Quizzes = _context.Quizzes.ToList();
            return View();
        }
        public IActionResult QuizEdit(int? SelectedQuizId)
        {
            var quizzes = _context.Quizzes.ToList();
            var allQuestions = _context.Questions.ToList();
            QuizModel? selectedQuiz = null;

            if (SelectedQuizId.HasValue)
            {
                selectedQuiz = _context.Quizzes
                    .Include(q => q.QuestionIds) // or QuizQuestions, depending on your model
                    .FirstOrDefault(q => q.QuizModelId == SelectedQuizId.Value);
            }

            var model = new QuizEdit
            {
                Quizzes = quizzes,
                SelectedQuiz = selectedQuiz,
                AllQuestions = allQuestions
            };

            return View(model);
        }
        public IActionResult QuestionEdit(int? SelectedQuestionId)
        {
            var questions = _context.Questions.ToList();
            Question? selectedQuestion = null;

            if (SelectedQuestionId.HasValue)
            {
                selectedQuestion = _context.Questions.FirstOrDefault(q => q.QuestionId == SelectedQuestionId.Value);
            }

            var model = new QuestionEdit
            {
                Questions = questions,
                SelectedQuestion = selectedQuestion
            };

            return View(model);
        }
        public IActionResult InsightList()
        {
            var insights = _context.QuizReviews.ToList();
            return View(insights);
        }
        public IActionResult InsightHome()
        {
            return View();
        }
        public IActionResult InsightCreate(int? selectedQuizId)
        {
            ViewBag.Quizzes = _context.Quizzes.ToList();

            List<QuizAttempt> attempts = new();
            double? avg = null;
            int? high = null;
            int? low = null;
            List<int> informativeRatings = new();
            List<int> engagementRatings = new();
            List<string> notes = new();

            if (selectedQuizId.HasValue)
            {
                attempts = _context.QuizAttempts
                    .Where(a => a.QuizID == selectedQuizId.Value)
                    .ToList();

                if (attempts.Any())
                {
                    avg = attempts.Average(a => a.Score);
                    high = attempts.Max(a => a.Score);
                    low = attempts.Min(a => a.Score);

                    informativeRatings = attempts
                        .Where(a => a.InformativeRating != 0) // Fix: Removed HasValue check
                        .Select(a => a.InformativeRating)
                        .ToList();

                    engagementRatings = attempts
                        .Where(a => a.EngagementRating != 0) // Fix: Removed HasValue check
                        .Select(a => a.EngagementRating)
                        .ToList();

                    notes = attempts
                        .Where(a => !string.IsNullOrWhiteSpace(a.Notes))
                        .Select(a => a.Notes)
                        .ToList();
                }
            }

            ViewBag.Attempts = attempts;
            ViewBag.Average = avg;
            ViewBag.Highest = high;
            ViewBag.Lowest = low;
            ViewBag.SelectedQuizId = selectedQuizId;
            ViewBag.InformativeRatings = informativeRatings;
            ViewBag.EngagementRatings = engagementRatings;
            ViewBag.Notes = notes;

            return View();
        }
        public IActionResult InsightEdit(int id)
        {
            var insight = _context.QuizReviews.FirstOrDefault(r => r.QuizReviewId == id);
            if (insight == null)
                return NotFound();
            return View(insight);
        }

        //for QuizReview db seeding, don't worry about it
        //public IActionResult SeedQuizReviews()
        //{
        //    if (!_context.QuizReviews.Any())
        //    {
        //        _context.QuizReviews.Add(new QuizReview
        //        {
        //            QuizId = 1,
        //            Tag = "Test Insight",
        //            AverageScore = 80,
        //            HighestScore = 95,
        //            LowestScore = 60,
        //            InformativeRatings = new List<int> { 5, 4, 4 },
        //            EngagementRatings = new List<int> { 4, 3, 5 },
        //            Comments = new List<string> { "Well structured.", "Enjoyed the quiz." }
        //        });

        //        _context.QuizReviews.Add(new QuizReview
        //        {
        //            QuizId = 2,
        //            Tag = "Test Insight 2",
        //            AverageScore = 50,
        //            HighestScore = 70,
        //            LowestScore = 35,
        //            InformativeRatings = new List<int> { 5, 4, 4 },
        //            EngagementRatings = new List<int> { 4, 3, 5 },
        //            Comments = new List<string> { "Quiz was hard.", "I am crashing out" }
        //        });

        //        // Add another row of dummy data
        //        _context.QuizReviews.Add(new QuizReview
        //        {
        //            QuizId = 3,
        //            Tag = "Sample Insight 3",
        //            AverageScore = 88,
        //            HighestScore = 100,
        //            LowestScore = 70,
        //            InformativeRatings = new List<int> { 5, 5, 4 },
        //            EngagementRatings = new List<int> { 5, 4, 5 },
        //            Comments = new List<string> { "Excellent quiz!", "Very engaging." }
        //        });

        //        _context.SaveChanges();
        //    }
        //    return Content("Dummy QuizReview seeded.");
        //}
        //function 1: connection string to the AWS Account
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
        [ValidateAntiForgeryToken]
        public IActionResult QuizCreate(QuizModel model, List<int>? QuestionIds)
        {
            // Assign selected questions to the quiz
            if (QuestionIds != null && QuestionIds.Any())
            {
                model.QuestionIds = _context.Questions
                    .Where(q => QuestionIds.Contains(q.QuestionId))
                    .ToList();
            }

            _context.Quizzes.Add(model);
            _context.SaveChanges();

            return RedirectToAction("QuizCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QuizEdit(int QuizModelId, string Title, List<int> QuestionIds)
        {
            var quiz = _context.Quizzes
                .Include(q => q.QuestionIds)
                .FirstOrDefault(q => q.QuizModelId == QuizModelId);

            if (quiz == null)
                return NotFound();

            quiz.Title = Title;

            // Remove all current questions from this quiz
            foreach (var q in quiz.QuestionIds)
            {
                q.QuizModelId = null;
            }

            // Assign new questions to this quiz
            var selectedQuestions = _context.Questions
                .Where(q => QuestionIds.Contains(q.QuestionId))
                .ToList();

            foreach (var q in selectedQuestions)
            {
                q.QuizModelId = quiz.QuizModelId;
            }

            quiz.QuestionIds = selectedQuestions;
            _context.SaveChanges();

            return RedirectToAction("QuizEdit", new { SelectedQuizId = QuizModelId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuestionCreate(Question model, int? SelectedQuizId)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Quizzes = await _context.Quizzes.ToListAsync();
                return View(model);
            }

            // Optional: validate that all four options are filled
            if (string.IsNullOrWhiteSpace(model.OptionOne) ||
                string.IsNullOrWhiteSpace(model.OptionTwo) ||
                string.IsNullOrWhiteSpace(model.OptionThree) ||
                string.IsNullOrWhiteSpace(model.OptionFour))
            {
                ModelState.AddModelError(string.Empty, "Please fill in all four options.");
                ViewBag.Quizzes = await _context.Quizzes.ToListAsync();
                return View(model);
            }

            // Optional: validate Answer is within 0–3
            if (model.Answer < 0 || model.Answer > 3)
            {
                ModelState.AddModelError("Answer", "Answer must be between 0 (Option 1) and 3 (Option 4).");
                ViewBag.Quizzes = await _context.Quizzes.ToListAsync();
                return View(model);
            }

            // Assign to quiz if selected
            if (SelectedQuizId.HasValue)
            {
                model.QuizModelId = SelectedQuizId.Value;
            }

            _context.Questions.Add(model);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Question created successfully!";
            return RedirectToAction("QuestionCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QuestionEdit(int QuestionId, string QuestionText, string OptionOne, string OptionTwo, string OptionThree, string OptionFour, int Answer, int Score)
        {
            var question = _context.Questions.FirstOrDefault(q => q.QuestionId == QuestionId);
            if (question == null)
                return NotFound();

            // Update properties
            question.QuestionText = QuestionText;
            question.OptionOne = OptionOne;
            question.OptionTwo = OptionTwo;
            question.OptionThree = OptionThree;
            question.OptionFour = OptionFour;
            question.Answer = Answer;
            question.Score = Score;

            _context.SaveChanges();

            // Optionally, redirect back to the edit page with the same question selected
            return RedirectToAction("QuestionEdit", new { SelectedQuestionId = QuestionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuiz(int QuizModelId)
        {
            var quiz = _context.Quizzes
                .Include(q => q.QuestionIds)
                .FirstOrDefault(q => q.QuizModelId == QuizModelId);

            if (quiz == null)
                return NotFound();

            // Optionally, remove related questions or set their QuizModelId to null
            foreach (var question in quiz.QuestionIds)
            {
                question.QuizModelId = null;
            }

            _context.Quizzes.Remove(quiz);
            _context.SaveChanges();

            // Redirect to QuizHome instead of QuizList
            return RedirectToAction("QuizEdit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuestion(int QuestionId)
        {
            var question = _context.Questions.FirstOrDefault(q => q.QuestionId == QuestionId);

            if (question == null)
                return NotFound();

            _context.Questions.Remove(question);
            _context.SaveChanges();

            // Redirect to the QuestionEdit page (or wherever you want)
            return RedirectToAction("QuestionEdit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsightCreate(
            int QuizId,
            string Tag,
            double? AverageScore,
            int? HighestScore,
            int? LowestScore)
        {
            // Extract attempts for the quiz
            var attempts = _context.QuizAttempts
                .Where(a => a.QuizID == QuizId)
                .ToList();

            var informativeRatings = attempts
                .Where(a => a.InformativeRating != 0)
                .Select(a => a.InformativeRating)
                .ToList();

            var engagementRatings = attempts
                .Where(a => a.EngagementRating != 0)
                .Select(a => a.EngagementRating)
                .ToList();

            var notes = attempts
                .Where(a => !string.IsNullOrWhiteSpace(a.Notes))
                .Select(a => a.Notes)
                .ToList();

            var insight = new QuizReview
            {
                QuizId = QuizId,
                Tag = Tag,
                AverageScore = AverageScore,
                HighestScore = HighestScore,
                LowestScore = LowestScore,
                InformativeRatings = informativeRatings,
                EngagementRatings = engagementRatings,
                Comments = notes
            };

            // Save to database
            _context.QuizReviews.Add(insight);
            _context.SaveChanges();

            // ---- Upload insight as JSON to AWS S3 ----
            try
            {
                string json = JsonSerializer.Serialize(insight);
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

                var values = getValues(); // Reads from appsettings.json
                string accessKey = values[0];
                string secretKey = values[1];
                string sessionToken = values[2];

                const string BucketName = "didyoufall4it-infographic-bucket";

                using var s3Client = new AmazonS3Client(
                    accessKey,
                    secretKey,
                    sessionToken,
                    Amazon.RegionEndpoint.USEast1);

                var uploadKey = $"insights/quiz_{QuizId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json";

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = uploadKey,
                    BucketName = BucketName,
                    ContentType = "application/json",
                    CannedACL = S3CannedACL.PublicRead
                };

                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);

                Console.WriteLine("Insight uploaded to S3 successfully.");

                // ---- Publish notification to AWS SNS ----
                using var snsClient = new AmazonSimpleNotificationServiceClient(
                    accessKey,
                    secretKey,
                    sessionToken,
                    Amazon.RegionEndpoint.USEast1);

                var publishRequest = new PublishRequest
                {
                    TopicArn = "arn:aws:sns:us-east-1:067385453713:NewInsightTopic", 
                    Subject = "New Insight Created",
                    Message = JsonSerializer.Serialize(new
                    {
                        QuizId = insight.QuizId,
                        Tag = insight.Tag,
                        S3Key = uploadKey,
                        Timestamp = DateTime.UtcNow
                    })
                };

                var snsResponse = await snsClient.PublishAsync(publishRequest);
                Console.WriteLine("SNS notification published: " + snsResponse.MessageId);
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"AWS S3 Upload Error: {ex.Message}");
                TempData["Error"] = "Insight creation failed during S3 upload.";
                return RedirectToAction("InsightList");
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                Console.WriteLine($"AWS SNS Error: {ex.Message}");
                TempData["Error"] = "Insight created, but SNS notification failed.";
                return RedirectToAction("InsightList");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                TempData["Error"] = "Unexpected error occurred during upload.";
                return RedirectToAction("InsightList");
            }

            TempData["Message"] = "Insight created, uploaded to S3, and SNS notified!";
            return RedirectToAction("InsightList");
        }




        // POST: Update the insight
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsightEdit(QuizReview model)
        {
            var insight = _context.QuizReviews.FirstOrDefault(r => r.QuizReviewId == model.QuizReviewId);
            if (insight == null)
                return NotFound();

            // Only update the Tag
            insight.Tag = model.Tag;

            _context.SaveChanges();
            return RedirectToAction("InsightList");
        }
        // POST: Delete the insight
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteInsight(int QuizReviewId)
        {
            var insight = _context.QuizReviews.FirstOrDefault(r => r.QuizReviewId == QuizReviewId);
            if (insight == null)
                return NotFound();

            _context.QuizReviews.Remove(insight);
            _context.SaveChanges();
            return RedirectToAction("InsightList");
        }
    }
}
