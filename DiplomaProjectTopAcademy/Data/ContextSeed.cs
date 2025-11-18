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
        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            // Читаем данные из конфигурации (appsettings.json или секретов)
            var email = config["SeedSuperAdmin:Email"];
            var username = config["SeedSuperAdmin:UserName"];
            var password = config["SeedSuperAdmin:Password"];

            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = username,
                Email = email,
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SubscriptionType = "Infinity",
                SubscriptionStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SubscriptionEndDate = DateTime.MaxValue, // 31 декабря 9999 года
                IsActive = true
            };
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // создаём суперадмина
                await userManager.CreateAsync(defaultUser, password);
                await userManager.AddToRoleAsync(defaultUser, Enums.Roles.SuperAdmin.ToString());
            }
            else
            {
                // обновляем подписку, если пользователь уже существует
                user.SubscriptionType = "Infinity";
                user.SubscriptionStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                user.SubscriptionEndDate = DateTime.MaxValue;
                user.IsActive = true;
                await userManager.UpdateAsync(user);
            }
        }
    }
}
