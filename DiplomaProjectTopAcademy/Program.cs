using DiplomaProjectTopAcademy.Controllers;
using DiplomaProjectTopAcademy.Data;
using DiplomaProjectTopAcademy.Models;
using DiplomaProjectTopAcademy.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ===== DbContext =====
// ConfigureServices method:
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// ===== Identity =====
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultUI()
        .AddDefaultTokenProviders();

// заменяем стандартный SignInManager на кастомный
builder.Services.AddScoped<SignInManager<ApplicationUser>, CustomSignInManager>();

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ===== Hosted Services =====
// HostedService для авто-бэкапа
builder.Services.AddHostedService<BackupHostedService>();
builder.Services.AddScoped<BackupController>();

// Регистрируем Hosted Service для проверки подписок
builder.Services.AddHostedService<SubscriptionCheckService>();

// ===== SecurityStampValidator =====
// SecurityStampValidator — чтобы кука не инвалидировалась мгновенно
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromHours(1);
});

/*
//***** Настройка общего Cookie-аутентификации ************
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "YourApp.Auth"; // Установка имени cookie (одинаково для всех микросервисов)
        options.LoginPath = "/Identity/Account/Login"; // Путь для перенаправления на страницу входа
        options.AccessDeniedPath = "/Identity/Account/AccessDenied"; //Путь для перенаправления при отказе в доступе
    });
//**********************************************************
*/


/*
// ***** Cookies (UI) + JWT (API) *****
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".AspNetCore.Identity.Application"; // или убери вовсе, чтобы было дефолтное имя
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
    })
    .AddJwtBearer("MicroserviceJwt", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
*/
// **************************************


// ===== JwtTokenService =====
builder.Services.AddScoped<JwtTokenService>();

// ===== Authentication =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".AspNetCore.Identity.Application";
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(2); // access‑токен живёт 1 минуту
        options.SlidingExpiration = false; // не продлеваем автоматически
    })
    .AddJwtBearer("MicroserviceJwt", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role // важно для работы ролей
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = ctx =>
            {
                var subEnd = ctx.Principal?.FindFirst("subscription_end")?.Value;
                if (!string.IsNullOrEmpty(subEnd))
                {
                    if (DateTime.TryParseExact(
                        subEnd,
                        "o",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                        out var endUtc))
                    {
                        if (endUtc <= DateTime.UtcNow)
                            ctx.Fail("Subscription expired");
                    }
                    else
                    {
                        var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("Invalid subscription_end format in JWT: {Value}", subEnd);
                    }
                }
                return Task.CompletedTask;
            }
        };

    });

// ===== Session =====
// включаем сессию *************************
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//******************************************

var app = builder.Build();

//***********************************************************


// ===== Seed DB =====
//We insert the code to initialize (seed) the database data (*Вставляем код для инициализации (seed) данных БД*):
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await ContextSeed.SeedRolesAsync(userManager, roleManager);
        var config = services.GetRequiredService<IConfiguration>();

        //Method connection to add superadmin user to database
        await ContextSeed.SeedSuperAdminAsync(userManager, roleManager, config);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger("Program");
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

//***********************************************************

// ===== Pipeline =====
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();   // обязательно ДО app.UseEndpoints()

app.UseAuthentication(); // добавлено для аутентификации
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
