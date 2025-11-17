using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

public class CustomSignInManager : SignInManager<ApplicationUser>
{
    public CustomSignInManager(UserManager<ApplicationUser> userManager,
                               IHttpContextAccessor contextAccessor,
                               IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
                               IOptions<IdentityOptions> optionsAccessor,
                               ILogger<SignInManager<ApplicationUser>> logger,
                               IAuthenticationSchemeProvider schemes,
                               IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password,
        bool isPersistent, bool lockoutOnFailure)
    {
        var user = await UserManager.FindByNameAsync(userName);

        // Без этого кода вход возможен всегда, но с некоторыми ограничениями
        //if (user != null && !user.IsActive)
        //{
        //    return SignInResult.NotAllowed; // блокируем вход
        //}

        return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }
}
