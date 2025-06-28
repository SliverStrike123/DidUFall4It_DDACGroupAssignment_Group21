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

            return RedirectToAction("QuizList");
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
        public IActionResult InsightCreate(QuizReview model, string InformativeRatingsInput, string EngagementRatingsInput, string CommentsInput)
        {
            // Parse and assign the ratings/comments from the form inputs
            model.InformativeRatings = InformativeRatingsInput?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s.Trim()))
                .ToList() ?? new List<int>();

            model.EngagementRatings = EngagementRatingsInput?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s.Trim()))
                .ToList() ?? new List<int>();

            model.Comments = CommentsInput?
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList() ?? new List<string>();

            _context.QuizReviews.Add(model);
            _context.SaveChanges();

            return RedirectToAction("InsightHome");
        }
    }
}
