using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DiplomaProjectTopAcademy.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Moderator,Basic")] // ¬се роли, кроме Inactive
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //защитита от случаев, когда пользователь вручную получит роль Inactive после авторизации
            if (User.IsInRole("Inactive"))
            {
                return Forbid(); // »ли RedirectToAction("Blocked");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous] // —траница ошибок доступна всем
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
