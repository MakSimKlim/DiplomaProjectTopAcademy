using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DiplomaProjectTopAcademy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        // —тартова€ страница - доступна всем
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Inactive"))
            {
                return Forbid(); // или RedirectToAction("Blocked");
            }

            ApplicationUser? user = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                user = await _userManager.GetUserAsync(User);
            }

            // ѕередаЄм пользовател€ в модель Razor
            return View(user);
        }


        // —траница "ѕолитика конфиденциальности" - доступна всем
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // —траница " ак это работает" - доступна всем
        [AllowAnonymous]
        public IActionResult HowItWorks()
        {
            return View();
        }

        // эта часть кода заменена на JWT Bearer логику
        ////ѕеренаправление из Identity в BusinessLogic 
        //[Authorize] // “олько дл€ авторизованных
        //public IActionResult RedirectToBusiness()
        //{
        //    return Redirect("http://localhost:5002/"); // URL BusinessLogic
        //}

        // следующие страницы (actions) в контроллере будут доступны только дл€ авторизованных пользователей с определенными рол€ми (кроме Inactive)
        [Authorize(Roles = "SuperAdmin,Admin,Moderator,Basic")]
        public IActionResult Profile()
        {
            return View();
        }

        // —траница ошибок - доступна всем
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}