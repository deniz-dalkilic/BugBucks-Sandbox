using System.Text;
using BugBucks.Shared.Logging;
using BugBucks.Shared.VaultClient;
using BugBucks.Shared.VaultClient.Extensions;
using IdentityService.Domain.Models;
using IdentityService.Infrastructure.Services;
using OrderService.Domain.Models;
using OrderService.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.Infrastructure.Data;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddVaultClient();

// Configure global logger using shared logging library and override default providers
LoggerConfigurator.ConfigureLogger(builder.Configuration);
builder.Host.UseSerilog();

// Configure DbContext with MySQL using Pomelo
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure ASP.NET Core Identity with ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<OrderDbContext>()
    .AddDefaultTokenProviders();

// Configure authentication: JWT and Google external login
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    var jwtSection = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]))
    };
})
.AddGoogle(googleOptions =>
{
    var googleSection = builder.Configuration.GetSection("Authentication:Google");
    googleOptions.ClientId = googleSection["ClientId"];
    googleOptions.ClientSecret = googleSection["ClientSecret"];
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Correlation ID middleware for logging
app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
    using (LogContext.PushProperty("CorrelationId", correlationId))
    {
        await next();
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

AppDomain.CurrentDomain.ProcessExit += (s, e) => LoggerConfigurator.CloseLogger();
