using System.Text;
using BugBucks.Shared.Logging;
using BugBucks.Shared.VaultClient.Extensions;
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

// Configure authentication: JWT only (central IdentityService handles user management)
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "JwtBearer";
        options.DefaultChallengeScheme = "JwtBearer";
    })
    .AddJwtBearer("JwtBearer", options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var key = jwtSection["Key"];
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("JWT signing key is not configured.");
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

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
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