using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Mvc;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class InfographicControllerUser : Controller
    {
        public static List<InfographicFeedback> feedbackDb = new();

        public IActionResult ViewAll() => View(feedbackDb);
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(string infographicTitle, string comment)
        {
            feedbackDb.Add(new InfographicFeedback
            {
                Id = feedbackDb.Count + 1,
                UserId = "demo-user",
                InfographicTitle = infographicTitle,
                Comment = comment,
                PostedAt = DateTime.Now
            });
            return RedirectToAction("ViewAll");
        }

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
    }
}
