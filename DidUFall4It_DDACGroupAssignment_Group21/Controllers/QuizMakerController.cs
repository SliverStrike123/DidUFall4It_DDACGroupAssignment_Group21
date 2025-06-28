using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class QuizMakerController : Controller
    {
        private readonly DidUFall4It_DDACGroupAssignment_Group21Context _context;
        private readonly IWebHostEnvironment _environment;

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
            //if (SelectedQuizId.HasValue)
            //{
            //    model.QuizModelId = SelectedQuizId.Value;
            //}

            _context.Questions.Add(model);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Question created successfully!";
            return RedirectToAction("QuestionCreate");
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
