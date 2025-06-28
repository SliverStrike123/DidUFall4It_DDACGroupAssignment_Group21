using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class QuizController : Controller
    {
        private readonly DidUFall4It_DDACGroupAssignment_Group21Context _context;
        private readonly IWebHostEnvironment _environment;

        public QuizController(IWebHostEnvironment environment, DidUFall4It_DDACGroupAssignment_Group21Context context)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index() => View();
        public IActionResult Submit() => View();

        //[HttpPost]
        //public IActionResult SubmitQuiz(string question, string selectedAnswer)
        //{
        //    _context.Questions.Add()
        //    .Add(new QuizAttempt
        //    {
        //        Id = quizDb.Count + 1,
        //        UserId = "demo-user",
        //        Question = question,
        //        SelectedAnswer = selectedAnswer,
        //        AttemptDate = DateTime.Now
        //    });
        //    return RedirectToAction("History");
        //}

        //public IActionResult History()
        //{
        //    var userHistory = quizDb.Where(x => x.UserId == "demo-user").ToList();
        //    return View(userHistory);
        //}

        //[HttpPost]
        //public IActionResult UpdateNotes(int id, string notes)
        //{
        //    var record = quizDb.FirstOrDefault(x => x.Id == id);
        //    if (record != null && record.UserId == "demo-user") record.Notes = notes;
        //    return RedirectToAction("History");
        //}

        //[HttpPost]
        //public IActionResult Delete(int id)
        //{
        //    var record = quizDb.FirstOrDefault(x => x.Id == id);
        //    if (record != null && record.UserId == "demo-user") quizDb.Remove(record);
        //    return RedirectToAction("History");
        //}
    }
}
