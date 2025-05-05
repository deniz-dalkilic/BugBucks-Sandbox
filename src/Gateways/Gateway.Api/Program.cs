using System.Text;
using BugBucks.Shared.Logging.Extensions;
using BugBucks.Shared.Vault.Extensions;
using BugBucks.Shared.Vault.Services;
using BugBucks.Shared.Web.Extensions;
using Microsoft.IdentityModel.Tokens;
using Serilog;

// Configure builder and logging as early as possible
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppLogging(builder.Configuration, builder.Environment);

builder.Host.UseSerilog();

builder.Services.AddBugBucksWeb();

builder.Services.AddVaultClient();


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

app.UseSerilogRequestLogging();
app.UseBugBucksWeb();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map reverse proxy endpoints
app.MapReverseProxy();

// Retrieve Vault secrets using the application's DI container
using (var scope = app.Services.CreateScope())
{
    var vaultService = scope.ServiceProvider.GetRequiredService<IVaultService>();

    try
    {
        var mountPoint = builder.Configuration["Vault:MountPoint"];
        var secretPath = builder.Configuration["Vault:SecretPath"];
        var secrets = await vaultService.GetSecretsAsync(mountPoint, secretPath);

        // Update JWT key from Vault if available
        builder.Configuration["Jwt:Key"] =
            secrets.ContainsKey("JwtKey") ? secrets["JwtKey"] : builder.Configuration["Jwt:Key"];


        Log.Information("Vault secrets retrieved successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error retrieving Vault secrets for API Gateway.");
    }
}

app.Run();