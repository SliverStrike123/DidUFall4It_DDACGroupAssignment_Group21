using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Identity;
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

        public IActionResult QuizViewUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var quizzes = _context.Quizzes
                .OrderBy(q => q.Title)
                .ToList();

            var attemptedQuizIds = _context.QuizAttempts
                .Where(a => a.UserId == userId)
                .Select(a => a.QuizID)
                .ToHashSet();

            foreach (var quiz in quizzes)
            {
                quiz.hasAttempted = attemptedQuizIds.Contains(quiz.QuizModelId);
            }

            var quizViewModel = new QuizViewModel
            {
                Quizzes = quizzes
            };

            return View(quizViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitQuiz(QuizSubmissionViewModel submission)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var questions = _context.Questions
                .Where(q => q.QuizModelId == submission.QuizID)
                .ToList();

            int score = 0;
            int totalscore = 0;
            foreach (var question in questions)
            {
                totalscore += question.Score ?? 0; 
                if (submission.SubmittedAnswers.TryGetValue(question.QuestionId, out int answer) &&
                    question.Answer == answer)
                {
                    score += question.Score ?? 0;
                }
            }

            var attempt = new QuizAttempt
            {
                UserId = userId,
                QuizID = submission.QuizID,
                Score = score,
                TotalScore = totalscore,
                AttemptDate = DateTime.Now
            };

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            // Redirect to the rating form
            return RedirectToAction("RateQuizView", new { attemptId = attempt.Id, score = attempt.Score, totalScore = attempt.TotalScore });
        }
        public IActionResult ProgressAccess()
        {
            return RedirectToAction("List", "Progress");
        }
        [HttpGet]
        public IActionResult RateQuizView(int attemptId, int score, int totalScore)
        {
            var model = new QuizAttempt { Id = attemptId, Score = score, TotalScore = totalScore };
            return View(model); // Will only collect Rating + Notes in the form
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateQuiz(QuizAttempt model)
        {
            var attempt = await _context.QuizAttempts.FindAsync(model.Id);
            if (attempt == null) return NotFound();

            attempt.Notes = model.Notes;
            attempt.InformativeRating = model.InformativeRating;
            attempt.EngagementRating = model.EngagementRating;

            _context.QuizAttempts.Update(attempt);
            await _context.SaveChangesAsync();

            return RedirectToAction("QuizViewUser");
        }
        public async Task<IActionResult> InfoViewAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var infographics = await _context.Infographics
                .OrderBy(i => i.Title)
                .ToListAsync();

            var feedbacks = await _context.InfographicFeedback
                .Where(f => f.UserId == userId)
                .ToListAsync();

            // Mark HasRated for each infographic
            foreach (var info in infographics)
            {
                var matchingFeedback = feedbacks.FirstOrDefault(f => f.InfographicId == info.Id);
                info.HasRated = matchingFeedback != null;

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
        [HttpPost]
        public async Task<IActionResult> Rate(InfographicFeedback model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // Prevent duplicate feedback from the same user
            bool alreadyRated = await _context.InfographicFeedback
                .AnyAsync(f => f.UserId == userId && f.InfographicId == model.InfographicId);

            if (alreadyRated)
            {
                TempData["Message"] = "You have already rated this infographic.";
                return RedirectToAction("InfoViewAll");
            }
            var infographic = await _context.Infographics
                .FirstOrDefaultAsync(i => i.Id == model.InfographicId);

            if (infographic == null)
            {
                TempData["Message"] = "Invalid infographic.";
                return RedirectToAction("InfoViewAll");
            }


            model.UserId = userId;
            model.PostedAt = DateTime.Now;
            model.InfographicTitle = infographic.Title;

            _context.InfographicFeedback.Add(model);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Thank you for your feedback!";
            return RedirectToAction("InfoViewAll");
        }
        // View all feedbacks by logged-in user
        public async Task<IActionResult> InfoMyFeedback()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var feedbacks = await _context.InfographicFeedback
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.PostedAt)
                .ToListAsync();

            return View(feedbacks);
        }

        // GET: Edit feedback
        [HttpGet]
        public async Task<IActionResult> InfoEditFeedbackView(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var feedback = await _context.InfographicFeedback
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (feedback == null)
                return NotFound();

            return View(feedback);
        }

        // POST: Save feedback
        [HttpPost]
        public async Task<IActionResult> EditFeedback(InfographicFeedback updated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var feedback = await _context.InfographicFeedback
                .FirstOrDefaultAsync(f => f.Id == updated.Id && f.UserId == userId);

            if (feedback == null)
                return NotFound();

            // Update fields
            feedback.InformativeRating = updated.InformativeRating;
            feedback.EngagementRating = updated.EngagementRating;
            feedback.ClarityRating = updated.ClarityRating;
            feedback.RelevanceRating = updated.RelevanceRating;
            feedback.Comment = updated.Comment;
            feedback.PostedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction("InfoMyFeedback");
        }

        // DELETE
        [HttpPost]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var feedback = await _context.InfographicFeedback
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (feedback != null)
            {
                _context.InfographicFeedback.Remove(feedback);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("InfoMyFeedback");
        }

        public static List<LearningGoal> goalDb = new();

        public IActionResult List()
        {
            var userGoals = goalDb.Where(x => x.UserId == "demo-user").ToList();
            return View(userGoals);
        }

        public IActionResult QuestionViewUser(int quizModelId)
        {
            var quiz = _context.Quizzes
                .Include(q => q.QuestionIds) // Include related questions
                .FirstOrDefault(q => q.QuizModelId == quizModelId);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        public async Task<IActionResult> QuizHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var history = await (
                from attempt in _context.QuizAttempts
                join quiz in _context.Quizzes on attempt.QuizID equals quiz.QuizModelId
                where attempt.UserId == userId
                orderby attempt.AttemptDate descending
                select new QuizHistoryViewModel
                {
                    QuizTitle = quiz.Title,
                    Score = attempt.Score,
                    AttemptDate = attempt.AttemptDate
                }
            ).ToListAsync();

            return View(history);
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

