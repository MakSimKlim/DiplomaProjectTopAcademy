using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaProjectTopAcademy.Controllers
{
    [Authorize(Roles = "SuperAdmin")] // доступ только суперадмину
    public class SubscriptionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Выбор тарифа
        [HttpPost]
        public async Task<IActionResult> ChoosePlan(string plan)
        {
            var user = await _userManager.GetUserAsync(User);
            switch (plan)
            {
                case "Trial":
                    if (!user.TrialUsed)
                    {
                        user.SubscriptionType = "Trial";
                        user.SubscriptionStartDate = DateTime.UtcNow;
                        user.SubscriptionEndDate = DateTime.UtcNow.AddDays(7);
                        user.TrialUsed = true;
                        user.IsActive = true;
                    }
                    break;

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

        // Продление подписки (пока заглушка вместо оплаты)
        [HttpPost]
        public async Task<IActionResult> ExtendSubscription(string plan)
        {
            var user = await _userManager.GetUserAsync(User);
            if (plan == "Monthly")
                user.SubscriptionEndDate = DateTime.UtcNow.AddMonths(1);
            else if (plan == "Yearly")
                user.SubscriptionEndDate = DateTime.UtcNow.AddYears(1);
            else if (plan == "Test")
                user.SubscriptionEndDate = DateTime.UtcNow.AddMinutes(1);

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Home");
        }
    }
}
