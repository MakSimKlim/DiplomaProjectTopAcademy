using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiplomaProjectTopAcademy.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class SubscriptionModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser AppUser { get; set; }

        public async Task OnGetAsync()
        {
            AppUser = await _userManager.GetUserAsync(User);
        }

        public async Task<IActionResult> OnPostExtendAsync(string plan)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Если супер‑админ — делаем вечную подписку
            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                user.SubscriptionType = "SuperAdmin";
                user.SubscriptionStartDate = DateTime.UtcNow;
                user.SubscriptionEndDate = null; // бесконечно
                user.IsActive = true;

                await _userManager.UpdateAsync(user);
                return RedirectToPage();
            }

            // обычная логика для остальных

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
            return RedirectToPage(); // остаёмся на Subscription.cshtml
        }

        public async Task<IActionResult> OnPostChooseAsync(string plan)
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
            return RedirectToPage(); // остаёмся на Subscription.cshtml
        }
    }
}
