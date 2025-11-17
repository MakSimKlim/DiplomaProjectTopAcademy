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
                user.SubscriptionType = "Infinity";
                user.SubscriptionStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                user.SubscriptionEndDate = DateTime.MaxValue; // 31 декабря 9999 года
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
                    ? currentEnd.AddMinutes(2)
                    : DateTime.UtcNow.AddMinutes(2);

            user.SubscriptionType = plan;
            user.IsActive = true;

            await _userManager.UpdateAsync(user);
            return RedirectToPage(); // остаёмся на Subscription.cshtml
        }
      
    }
}
