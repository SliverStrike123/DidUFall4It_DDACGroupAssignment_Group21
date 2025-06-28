using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class QuizMakerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult QuizHome()
        {
            return View();
        }

        public IActionResult QuizList()
        {
            return View();
        }

        public IActionResult QuizCreate()
        {
            return View();
        }
        public IActionResult QuestionCreate()
        {
            return View();
        }
        public IActionResult QuizEdit()
        {
            return View();
        }
        public IActionResult QuestionEdit()
        {
            return View();
        }
        public IActionResult InsightList()
        {
            return View();
        }
        public IActionResult InsightHome()
        {
            return View();
        }
        public IActionResult InsightCreate()
        {
            return View();
        }
        public IActionResult InsightEdit()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteQuiz(int QuizModelId)
        //{
        //    var quiz = _context.Quizzes.Find(QuizModelId);
        //    if (quiz != null)
        //    {
        //        _context.Quizzes.Remove(quiz);
        //        _context.SaveChanges();
        //    }
        //    return RedirectToAction("QuizEdit");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteQuestion(int QuestionId)
        //{
        //    var question = _context.Questions.Find(QuestionId);
        //    if (question != null)
        //    {
        //        _context.Questions.Remove(question);
        //        _context.SaveChanges();
        //    }
        //    return RedirectToAction("QuestionEdit");
        //}
    }
}
