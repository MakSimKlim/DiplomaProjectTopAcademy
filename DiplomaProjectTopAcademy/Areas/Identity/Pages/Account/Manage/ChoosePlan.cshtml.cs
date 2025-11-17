using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace DiplomaProjectTopAcademy.Areas.Identity.Pages.Account.Manage
{
    public class ChoosePlanModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChoosePlanModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string Plan { get; set; }
        [BindProperty] 
        public string Mode { get; set; } // "choose" или "extend"

        public void OnGet(string plan, string mode)
        {
            // сохраняем выбранный план из querystring
            Plan = plan;
            Mode = mode ?? "choose";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var now = DateTime.UtcNow;
            var currentEnd = user.SubscriptionEndDate ?? now;

            if (Mode == "extend")
            {
                // добавляем к оставшемуся времени
                switch (Plan)
                {
                    case "Monthly":
                        user.SubscriptionEndDate = currentEnd > now
                            ? currentEnd.AddMonths(1)
                            : now.AddMonths(1);
                        break;
                    case "Yearly":
                        user.SubscriptionEndDate = currentEnd > now
                            ? currentEnd.AddYears(1)
                            : now.AddYears(1);
                        break;
                    case "Test":
                        user.SubscriptionEndDate = currentEnd > now
                            ? currentEnd.AddMinutes(2)
                            : now.AddMinutes(2);
                        break;
                }
            }
            else
            {
                // новая подписка (обнуляем и ставим заново)
                user.SubscriptionStartDate = now;
                switch (Plan)
                {
                    case "Monthly":
                        user.SubscriptionEndDate = now.AddMonths(1);
                        break;
                    case "Yearly":
                        user.SubscriptionEndDate = now.AddYears(1);
                        break;
                    case "Test":
                        user.SubscriptionEndDate = now.AddMinutes(2);
                        break;
                }
            }

            user.SubscriptionType = Plan;
            user.IsActive = true;

            await _userManager.UpdateAsync(user);

            TempData["Message"] = $"План {Plan} успешно {(Mode == "extend" ? "продлён" : "активирован")}!";
            return RedirectToPage("/Account/Manage/Subscription");

        }
    }
}
