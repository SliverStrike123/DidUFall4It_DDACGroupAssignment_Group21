using Microsoft.AspNetCore.Mvc;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class UserController : Controller
    {
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
    }
}

