using System.Text;
using BugBucks.Shared.Logging.Extensions;
using BugBucks.Shared.VaultClient.Extensions;
using IdentityService.Api.Authorization;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Services;
using IdentityService.Domain.Models;
using IdentityService.Infrastructure.Data;
using IdentityService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddVaultClient();

builder.AddAppLogging();

// Configure DbContext with MySQL (Pomelo)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure ASP.NET Core Identity with ApplicationUser using integer primary key
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT authentication (using IdentityService JWT settings)
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "JwtBearer";
        options.DefaultChallengeScheme = "JwtBearer";
    })
    .AddJwtBearer("JwtBearer", options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var key = jwtSection["Key"];
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException("JWT signing key is not configured.");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency injection for application services and repository
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();

// Register the custom authorization policy and handler before building the app
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOrAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new OwnerOrAdminRequirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, OwnerOrAdminHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed default roles (User, Admin, Manager, Support, Customer)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    var roleNames = new[] { "User", "Admin", "Manager", "Support", "Customer" };

    foreach (var roleName in roleNames)
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole<int>(roleName));
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