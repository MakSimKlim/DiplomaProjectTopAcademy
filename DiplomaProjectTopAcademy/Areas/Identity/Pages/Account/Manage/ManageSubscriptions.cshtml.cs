using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiplomaProjectTopAcademy.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "SuperAdmin")]
    public class ManageSubscriptionsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageSubscriptionsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<ApplicationUser> Users { get; set; }

        public void OnGet()
        {
            Users = _userManager.Users.ToList();
        }

        public async Task<IActionResult> OnPostSetSubscriptionAsync(string userId, string plan, DateTime? endDate)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Если супер‑админ — вечная подписка
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

            user.SubscriptionType = plan;
            user.SubscriptionStartDate = DateTime.UtcNow;
            user.SubscriptionEndDate = endDate ?? DateTime.UtcNow.AddMonths(1);
            user.IsActive = true;

            await _userManager.UpdateAsync(user);

            return RedirectToPage(); // остаёмся на ManageSubscriptions.cshtml
        }
    }
}
