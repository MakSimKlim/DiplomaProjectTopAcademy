using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaProjectTopAcademy.Controllers
{
    [Authorize] // доступ для авторизованных пользователей
    public class SubscriptionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: страница выбора тарифа
        [HttpGet]
        public IActionResult ChoosePlan()
        {
            return View(); // отобразит Views/Subscription/ChoosePlan.cshtml
        }

        // POST: активация тарифа
        [HttpPost]
        public async Task<IActionResult> ChoosePlan(string plan)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            switch (plan)
            {
                case "Monthly":
                    user.SubscriptionType = "Monthly";
                    user.SubscriptionStartDate = DateTime.UtcNow;
                    user.SubscriptionEndDate = DateTime.UtcNow.AddMonths(1);
                    user.IsActive = true;
                    break;

                case "Yearly":
                    user.SubscriptionType = "Yearly";
                    user.SubscriptionStartDate = DateTime.UtcNow;
                    user.SubscriptionEndDate = DateTime.UtcNow.AddYears(1);
                    user.IsActive = true;
                    break;

                case "Test":
                    user.SubscriptionType = "Test";
                    user.SubscriptionStartDate = DateTime.UtcNow;
                    user.SubscriptionEndDate = DateTime.UtcNow.AddMinutes(1);
                    user.IsActive = true;
                    break;
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "Home");
        }

        // Продление подписки
        [HttpPost]
        public async Task<IActionResult> ExtendSubscription(string plan)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var currentEnd = user.SubscriptionEndDate ?? DateTime.UtcNow;

            if (plan == "Monthly")
                user.SubscriptionEndDate = currentEnd > DateTime.UtcNow
                    ? currentEnd.AddMonths(1)
                    : DateTime.UtcNow.AddMonths(1);
            else if (plan == "Yearly")
                user.SubscriptionEndDate = currentEnd > DateTime.UtcNow
                    ? currentEnd.AddYears(1)
                    : DateTime.UtcNow.AddYears(1);
            else if (plan == "Test")
                user.SubscriptionEndDate = currentEnd > DateTime.UtcNow
                    ? currentEnd.AddMinutes(1)
                    : DateTime.UtcNow.AddMinutes(1);

            user.SubscriptionType = plan;
            user.IsActive = true;

            await _userManager.UpdateAsync(user);
            //return RedirectToAction("Index", "Home");
            return RedirectToPage("/Subscription");
        }

        // Методы для суперАдмина
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult ManageSubscriptions()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
    }
}
