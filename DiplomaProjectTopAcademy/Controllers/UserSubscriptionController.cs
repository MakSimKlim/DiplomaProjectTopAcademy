using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaProjectTopAcademy.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UserSubscriptionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSubscriptionController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users); // передаём список пользователей в представление
        }

        [HttpPost]
        public async Task<IActionResult> SetSubscription(string userId, string plan, DateTime? endDate)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Если супер‑админ — вечная подписка
            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                user.SubscriptionType = "Infinity";
                user.SubscriptionStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                user.SubscriptionEndDate = DateTime.MaxValue;
                user.IsActive = true;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }

            // обычная логика для остальных
            user.SubscriptionType = plan;
            user.SubscriptionStartDate = DateTime.UtcNow;

            if (plan == "Trial" && !user.TrialUsed)
            {
                user.SubscriptionEndDate = DateTime.UtcNow.AddDays(7);
                user.TrialUsed = true;
                user.IsActive = true;
            }
            else if (plan == "Test")
            {
                user.SubscriptionEndDate = DateTime.UtcNow.AddMinutes(1);
                user.IsActive = true;
            }
            else if (plan == "Monthly")
            {
                user.SubscriptionEndDate = DateTime.UtcNow.AddMonths(1);
                user.IsActive = true;
            }
            else if (plan == "Yearly")
            {
                user.SubscriptionEndDate = DateTime.UtcNow.AddYears(1);
                user.IsActive = true;
            }
            else
            {
                //user.SubscriptionEndDate = endDate ?? DateTime.UtcNow.AddMonths(1);
                user.SubscriptionType = "Inactive";
                user.SubscriptionEndDate = DateTime.UtcNow; // подписка сразу считается истекшей
                user.IsActive = false;
            }
           
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
