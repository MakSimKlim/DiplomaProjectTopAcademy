using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    }
}
