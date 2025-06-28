using DidUFall4It_DDACGroupAssignment_Group21.Areas.Identity.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DidUFall4It_DDACGroupAssignment_Group21.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<DidUFall4It_DDACGroupAssignment_Group21User> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<DidUFall4It_DDACGroupAssignment_Group21User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.UserRole))
                        return RedirectToAction("Index", "Home"); // Default fallback
                    else if (user.UserRole == "Infographic")
                        return RedirectToAction("Index", "Infographic");
                    else if (user.UserRole == "QuizMaker")
                        return RedirectToAction("Index", "QuizMaker");
                    else if (user.UserRole == "User")
                        return RedirectToAction("Index", "User");
                    else
                        return Redirect("~/Identity/Account/Manage/Index");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
