using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Encodings.Web;

namespace DiplomaProjectTopAcademy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;   // добавляем поле

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _logger = logger;
            _userManager = userManager;
            _config = config;
        }

        // Стартовая страница - доступна всем
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

            // Передаём пользователя в модель Razor
            return View(user);
        }

        // переход в бизнес‑модуль по кнопке
        [Authorize(Roles = "SuperAdmin,Admin,Moderator,Basic")]
        public IActionResult RedirectToBusiness()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            var userId = HttpContext.Session.GetString("UserId");
            var businessUrl = _config["BusinessLogic:BaseUrl"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(userId))
            {
                return Content("JWT token is missing");
            }

            var redirectUrl = $"{businessUrl}?token={UrlEncoder.Default.Encode(accessToken)}&refresh={UrlEncoder.Default.Encode(refreshToken)}&userId={UrlEncoder.Default.Encode(userId)}";
            return Redirect(redirectUrl);
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

        // эта часть кода заменена на JWT Bearer логику
        ////Перенаправление из Identity в BusinessLogic 
        //[Authorize] // Только для авторизованных
        //public IActionResult RedirectToBusiness()
        //{
        //    return Redirect("http://localhost:5002/"); // URL BusinessLogic
        //}

        // следующие страницы (actions) в контроллере будут доступны только для авторизованных пользователей с определенными ролями (кроме Inactive)
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin,Admin,Moderator,Basic")]
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