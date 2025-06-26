using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Mvc;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class ProgressController : Controller
    {
        public static List<LearningGoal> goalDb = new();

        public IActionResult List()
        {
            var userGoals = goalDb.Where(x => x.UserId == "demo-user").ToList();
            return View(userGoals);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(string goal)
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
        public IActionResult Delete(int id)
        {
            var item = goalDb.FirstOrDefault(x => x.Id == id);
            if (item != null && item.UserId == "demo-user") goalDb.Remove(item);
            return RedirectToAction("List");
        }
    }

}
