using System.Text;
using BugBucks.Shared.Logging;
using BugBucks.Shared.Messaging.Implementations;
using BugBucks.Shared.Messaging.Interfaces;
using BugBucks.Shared.VaultClient.Extensions;
using IdentityService.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.Application.Interfaces;
using OrderService.Application.Services;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

// Load Vault secrets
builder.Services.AddVaultClient();

// Configure global logger using shared logging library and override default providers
LoggerConfigurator.ConfigureLogger(builder.Configuration);
builder.Host.UseSerilog();

// Configure DbContext with MySQL using Pomelo
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// Configure JWT authentication 
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

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderServiceImplementation>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOrAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new OwnerOrAdminRequirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, OwnerOrAdminHandler>();

var rabbitMqSection = builder.Configuration.GetSection("RabbitMQ");
var hostName = rabbitMqSection["HostName"] ?? "localhost";
var port = int.Parse(rabbitMqSection["Port"] ?? "5672");
var userName = rabbitMqSection["UserName"] ?? "guest";
var password = rabbitMqSection["Password"] ?? "guest";

builder.Services.AddSingleton<IRabbitMqPublisher>(sp =>
    RabbitMqPublisher.CreateAsync(hostName, port, userName, password).GetAwaiter().GetResult());

builder.Services.AddSingleton<IRabbitMqConsumer>(sp =>
    RabbitMqConsumer.CreateAsync(hostName, port, userName, password).GetAwaiter().GetResult());


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