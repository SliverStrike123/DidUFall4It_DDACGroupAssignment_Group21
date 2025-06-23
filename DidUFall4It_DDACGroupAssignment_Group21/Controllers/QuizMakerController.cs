using Microsoft.AspNetCore.Mvc;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class QuizMakerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
