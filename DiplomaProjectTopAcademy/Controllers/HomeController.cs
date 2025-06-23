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

        // Стартовая страница - доступна всем
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Дополнительная проверка для Inactive пользователей
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Inactive"))
            {
                return Forbid(); // Или RedirectToAction("Blocked");
            }
            return View();
        }

        // Страница "Политика конфиденциальности" - доступна всем
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // Страница "Как это работает" - доступна всем
        [AllowAnonymous]
        public IActionResult HowItWorks()
        {
            return View();
        }

        //Перенаправление из Identity в BusinessLogic
        [Authorize] // Только для авторизованных
        public IActionResult RedirectToBusiness()
        {
            return Redirect("http://localhost:5002/"); // URL BusinessLogic
        }

        // следующие страницы (actions) в контроллере будут доступны только для авторизованных пользователей с определенными ролями (кроме Inactive)
        [Authorize(Roles = "SuperAdmin,Admin,Moderator,Basic")]
        public IActionResult Profile()
        {
            return View();
        }

        // Страница ошибок - доступна всем
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}