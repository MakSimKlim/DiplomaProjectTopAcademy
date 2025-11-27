using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthJwtController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenService _jwtService;
    private readonly ILogger<AuthJwtController> _logger;

    public AuthJwtController(UserManager<ApplicationUser> userManager,
                             JwtTokenService jwtService,
                             ILogger<AuthJwtController> logger)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _logger = logger;
    }
    // ===== LOGIN =====
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            _logger.LogWarning("Login failed for {Email}", dto.Email);
            return Unauthorized();
        }

        var accessToken = await _jwtService.GenerateJwtToken(user, 2); // 2 минуты
        var refreshToken = _jwtService.GenerateRefreshToken();

        // сохраняем refresh‑токен в Identity
        await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);

        // Сохраняем токены в куки
        Response.Cookies.Append("BusinessJwt", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,                 // для локалки false, для прод true
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(2) // кука живёт 2 минуты
        });

        Response.Cookies.Append("BusinessRefresh", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,                 // для локалки false, для прод true
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        _logger.LogInformation("User {Email} logged in with JWT + Refresh.", dto.Email);

        return Ok(new
        {
            access_token = accessToken,
            refresh_token = refreshToken,
            user_id = user.Id
        });
    }

    // ===== REFRESH =====
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
        {
            _logger.LogWarning("Refresh failed: user not found");
            return Unauthorized();
        }

        var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
        if (storedToken != dto.RefreshToken)
        {
            _logger.LogWarning("Refresh failed: token mismatch");
            return Unauthorized();
        }

        // Проверка подписки
        if (user.SubscriptionEndDate == null || user.SubscriptionEndDate <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh failed: subscription expired for user {UserId}", user.Id);
            return Unauthorized();
        }

        var newAccessToken = await _jwtService.GenerateJwtToken(user, 2); // возвращаем 2 минуты при refresh
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", newRefreshToken);

        // обновляем куки
        Response.Cookies.Append("BusinessJwt", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(2) // возвращаем 2 минуты при refresh
        });

        Response.Cookies.Append("BusinessRefresh", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        _logger.LogInformation("Refresh successful for user {UserId}", user.Id);

        return Ok(new
        {
            access_token = newAccessToken,
            refresh_token = newRefreshToken
        });
    }

    // ===== LOGOUT =====
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
        }

        Response.Cookies.Delete("BusinessJwt");
        Response.Cookies.Delete("BusinessRefresh");
        Response.Cookies.Delete(".AspNetCore.Identity.Application");

        HttpContext.Session.Clear();

        _logger.LogInformation("User {User} logged out, cookies cleared, refresh token invalidated.", User.Identity?.Name);

        return Ok(new { message = "Logged out successfully" });
    }
    
}


// DTOs
public record LoginDto(string Email, string Password);
public record RefreshDto(string UserId, string RefreshToken);