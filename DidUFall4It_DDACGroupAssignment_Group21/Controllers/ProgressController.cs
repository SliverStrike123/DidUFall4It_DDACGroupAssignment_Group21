using DidUFall4It_DDACGroupAssignment_Group21.Areas.Identity.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class ProgressController : Controller
    {
        private readonly DidUFall4It_DDACGroupAssignment_Group21Context _context;
        private readonly UserManager<DidUFall4It_DDACGroupAssignment_Group21User> _userManager;

        public ProgressController(DidUFall4It_DDACGroupAssignment_Group21Context context, UserManager<DidUFall4It_DDACGroupAssignment_Group21User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var startDate = DateTime.Now;
            var duration = (endDate - startDate).Days;

            var newGoal = new LearningGoal
            {
                Goal = goal,
                UserId = userId,
                CreatedAt = startDate,
                EndDate = endDate,
                DurationDays = duration,
                IsCompleted = false
            };

            _context.LearningGoals.Add(newGoal);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Learning goal added successfully.";
            return View();  // Stay on the same page
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
                _context.LearningGoals.Remove(goal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }
    }

}
