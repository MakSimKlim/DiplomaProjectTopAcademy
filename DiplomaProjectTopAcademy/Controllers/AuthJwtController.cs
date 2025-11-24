using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DiplomaProjectTopAcademy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthJwtController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthJwtController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // ищем пользователя по email
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized();

            // роли из таблицы UserRoles
            var roles = await _userManager.GetRolesAsync(user);

            // базовые claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("role", string.Join(",", roles))
            };

            // добавляем роли как отдельные claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // кастомные claims из User
            if (!string.IsNullOrEmpty(user.SubscriptionType))
                claims.Add(new Claim("subscription_type", user.SubscriptionType));
            if (user.SubscriptionEndDate.HasValue)
                claims.Add(new Claim("subscription_exp", user.SubscriptionEndDate.Value.ToString("o")));

            // ключ и подпись
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),// - время действия токена
                signingCredentials: creds);

            // возвращаем токен клиенту
            return Ok(new { access_token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }

    public record LoginDto(string Email, string Password);
}