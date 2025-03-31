using System.Text;
using BugBucks.Shared.Logging;
using BugBucks.Shared.VaultClient;
using BugBucks.Shared.VaultClient.Extensions;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;

// Configure builder and logging as early as possible
var builder = WebApplication.CreateBuilder(args);
LoggerConfigurator.ConfigureLogger(builder.Configuration);
builder.Host.UseSerilog();

// Register services
builder.Services.AddVaultClient();
builder.Services.AddSingleton<IVaultClientService, VaultClientService>();
builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));

// Configure Authentication
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

// Configure Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiUserPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("User", "Admin", "Manager", "Support", "Customer");
    });
});

// Add YARP reverse proxy and load its configuration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Log startup
Log.Information("Application starting up...");

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// Use correlation ID middleware for logging
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

// Map reverse proxy endpoints
app.MapReverseProxy();

// Retrieve Vault secrets using the application's DI container
using (var scope = app.Services.CreateScope())
{
    var vaultService = scope.ServiceProvider.GetRequiredService<IVaultClientService>();
    var appLogger = scope.ServiceProvider.GetRequiredService<IAppLogger<Program>>();
    try
    {
        var mountPoint = builder.Configuration["Vault:MountPoint"];
        var secretPath = builder.Configuration["Vault:SecretPath"];
        var secrets = await vaultService.GetSecretsAsync(mountPoint, secretPath);

        // Update JWT key from Vault if available
        builder.Configuration["Jwt:Key"] =
            secrets.ContainsKey("JwtKey") ? secrets["JwtKey"] : builder.Configuration["Jwt:Key"];

        appLogger.LogInformation("Vault secrets retrieved successfully.");
    }
    catch (Exception ex)
    {
        appLogger.LogError(ex, "Error retrieving Vault secrets for API Gateway.");
    }
}

app.Run();

// Log shutdown on process exit
AppDomain.CurrentDomain.ProcessExit += (s, e) => LoggerConfigurator.CloseLogger();