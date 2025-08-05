using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DidUFall4It_DDACGroupAssignment_Group21.Areas.Identity.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text.Json;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class ProgressController : Controller
    {
        private readonly DidUFall4It_DDACGroupAssignment_Group21Context _context;
        private readonly UserManager<DidUFall4It_DDACGroupAssignment_Group21User> _userManager;

        private const string SnsTopicArn = "arn:aws:sns:us-east-1:600777367894:LearningGoalBroadcast";

        public ProgressController(
            DidUFall4It_DDACGroupAssignment_Group21Context context,
            UserManager<DidUFall4It_DDACGroupAssignment_Group21User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
  
        private List<string> getValues()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();
            return new List<string>
            {
                configure["Values:Key1"],
                configure["Values:Key2"],
                configure["Values:Key3"]
            };
        }

        public async Task<IActionResult> List()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var goals = await _context.LearningGoals
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
            return View(goals);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string goal, DateTime endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var userEmail = user?.Email ?? "unknown@example.com";
            var startDate = DateTime.Now;

            var newGoal = new LearningGoal
            {
                Goal = goal,
                UserId = userId,
                CreatedAt = startDate,
                EndDate = endDate,
                DurationDays = (endDate - startDate).Days,
                IsCompleted = false
            };

            _context.LearningGoals.Add(newGoal);
            await _context.SaveChangesAsync();

            await PublishToSNS(userEmail, goal, endDate, "Created");
            TempData["Message"] = "Goal created successfully!";
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var goal = await _context.LearningGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal != null)
            {
                goal.IsCompleted = true;
                await _context.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(userId);
                var userEmail = user?.Email ?? "unknown@example.com";
                await PublishToSNS(userEmail, goal.Goal, goal.EndDate, "Completed");
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var goal = await _context.LearningGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal != null)
            {
                string deletedGoal = goal.Goal;
                DateTime deletedEndDate = goal.EndDate;

                _context.LearningGoals.Remove(goal);
                await _context.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(userId);
                var userEmail = user?.Email ?? "unknown@example.com";
                await PublishToSNS(userEmail, deletedGoal, deletedEndDate, "Deleted");
            }

            return RedirectToAction("List");
        }

        private async Task PublishToSNS(string userEmail, string goal, DateTime endDate, string eventType)
        {
            try
            {
                var keys = getValues();
                using var snsClient = new AmazonSimpleNotificationServiceClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

                var messageJson = JsonSerializer.Serialize(new
                {
                    UserEmail = userEmail,
                    Goal = goal ?? "No goal provided",
                    EndDate = endDate != default ? endDate.ToString("yyyy-MM-dd") : "No end date",
                    EventType = eventType,
                    Timestamp = DateTime.UtcNow
                });

                var snsResponse = await snsClient.PublishAsync(new PublishRequest
                {
                    TopicArn = SnsTopicArn,
                    Subject = $"Learning Goal {eventType}",
                    Message = messageJson
                });

                System.Diagnostics.Debug.WriteLine($"[SNS] Published {eventType} event. MessageId: {snsResponse.MessageId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SNS ERROR] {ex.Message}");
            }
        }
    }
}
 