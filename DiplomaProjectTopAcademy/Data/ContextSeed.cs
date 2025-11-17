using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiplomaProjectTopAcademy.Models;

namespace DiplomaProjectTopAcademy.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Basic.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Inactive.ToString()));
        }
        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "superadmin",
                Email = "83maxomus@mail.ru",
                FirstName = "Max",
                LastName = "Klimov",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SubscriptionType = "Infinity",
                SubscriptionStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SubscriptionEndDate = DateTime.MaxValue, // 31 декабря 9999 года
                IsActive = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word.");
                    //await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Inactive.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Basic.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Moderator.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.SuperAdmin.ToString());
                }
                else
                {
                    // если пользователь уже есть, обновим подписку
                    user.SubscriptionType = "Infinity";
                    user.SubscriptionStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    user.SubscriptionEndDate = DateTime.MaxValue; // 31 декабря 9999 года
                    user.IsActive = true;
                    await userManager.UpdateAsync(user);
                }
            }
        }
    }
}
