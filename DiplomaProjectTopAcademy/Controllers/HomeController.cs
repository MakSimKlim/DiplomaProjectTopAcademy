using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DiplomaProjectTopAcademy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // —тартова€ страница - доступна всем
        [AllowAnonymous]
        public IActionResult Index()
        {
            // ƒополнительна€ проверка дл€ Inactive пользователей
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Inactive"))
            {
                return Forbid(); // »ли RedirectToAction("Blocked");
            }
            return View();
        }

        // —траница "ќ нас"/"ѕолитика" - доступна всем
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

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