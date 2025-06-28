using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class UserController : Controller
    {
        private readonly DidUFall4It_DDACGroupAssignment_Group21Context _context;
        private readonly IWebHostEnvironment _environment;

        public UserController(IWebHostEnvironment environment, DidUFall4It_DDACGroupAssignment_Group21Context context)
        {
            _context = context;
            _environment = environment;
        }
        public static List<InfographicFeedback> feedbackDb = new();
        public IActionResult Index()
        {
            return View(); 
        }

        public IActionResult UserHome()
        {
            return View();
        }

        public IActionResult QuizAccess()
        {
            return RedirectToAction("Index", "Quiz");
        }

        public IActionResult InfographicAccess()
        {
            return RedirectToAction("ViewAll", "InfographicControllerUser");
        }

        public IActionResult ProgressAccess()
        {
            return RedirectToAction("List", "Progress");
        }

        public async Task<IActionResult> InfoViewAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var infographics = await _context.Infographics
                .Include(i => i.InfographicFeedbacks)
                .ToListAsync();

            // Mark HasRated for each infographic
            foreach (var info in infographics)
            {
                info.InfographicFeedbacks = info.InfographicFeedbacks ?? new List<InfographicFeedback>();
                info.HasRated = info.InfographicFeedbacks.Any(f => f.UserId == userId); // You’ll add this prop below
            }

            var viewModel = new InfographicViewModel(infographics);
            return View(viewModel);
        }
        public IActionResult ViewInfo(int id)
        {
            var infographic = _context.Infographics.FirstOrDefault(i => i.Id == id);
            if (infographic == null)
            {
                return NotFound();
            }

            return View(infographic);
        }

        public IActionResult ViewRateInfo(int id)
        {
            // Ensure the infographic exists
            if (!_context.Infographics.Any(i => i.Id == id))
                return NotFound();

            var feedback = new InfographicFeedback
            {
                InfographicId = id
            };

            return View(feedback);
        }
        public async Task<bool> HasUserGivenFeedback(int infographicId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return false; // Not logged in or user ID not found

            var hasFeedback = await _context.InfographicFeedback
                .AnyAsync(f => f.InfographicId == infographicId && f.UserId == userId);

            return hasFeedback;
        }
        public IActionResult Create() => View();

        //[HttpPost]
        //public IActionResult CreateFeedback(string infographicTitle, string comment)
        //{
        //    feedbackDb.Add(new InfographicFeedback
        //    {
        //        Id = feedbackDb.Count + 1,
        //        UserId = "demo-user",
        //        InfographicTitle = infographicTitle,
        //        Comment = comment,
        //        PostedAt = DateTime.Now
        //    });
        //    return RedirectToAction("ViewAll");
        //}

        public IActionResult MyComments()
        {
            var userFeedback = feedbackDb.Where(x => x.UserId == "demo-user").ToList();
            return View(userFeedback);
        }

        [HttpPost]
        public IActionResult Update(int id, string comment)
        {
            var item = feedbackDb.FirstOrDefault(x => x.Id == id);
            if (item != null && item.UserId == "demo-user") item.Comment = comment;
            return RedirectToAction("MyComments");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = feedbackDb.FirstOrDefault(x => x.Id == id);
            if (item != null && item.UserId == "demo-user") feedbackDb.Remove(item);
            return RedirectToAction("MyComments");
        }

        public static List<LearningGoal> goalDb = new();

        public IActionResult List()
        {
            var userGoals = goalDb.Where(x => x.UserId == "demo-user").ToList();
            return View(userGoals);
        }

        public IActionResult CreateLearningGoalView() => View();

        [HttpPost]
        public IActionResult CreateLearningGoal(string goal)
        {
            goalDb.Add(new LearningGoal
            {
                Id = goalDb.Count + 1,
                UserId = "demo-user",
                Goal = goal,
                IsCompleted = false,
                CreatedAt = DateTime.Now
            });
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult MarkComplete(int id)
        {
            var item = goalDb.FirstOrDefault(x => x.Id == id);
            if (item != null && item.UserId == "demo-user") item.IsCompleted = true;
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult DeleteLearningGoal(int id)
        {
            var item = goalDb.FirstOrDefault(x => x.Id == id);
            if (item != null && item.UserId == "demo-user") goalDb.Remove(item);
            return RedirectToAction("List");
        }
    }
}

