using BugBucks.Shared.Logging.Extensions;
using BugBucks.Shared.Messaging.Consumers;
using BugBucks.Shared.Messaging.Extensions;
using BugBucks.Shared.Messaging.Interfaces;
using BugBucks.Shared.Messaging.Publishers;
using CheckoutService.Application.Interfaces;
using CheckoutService.Application.Services;
using CheckoutService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

//using CheckoutService.Infrastructure.Data;
//using CheckoutService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.AddAppLogging();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddScoped<ICheckoutService, CheckoutServiceImplementation>();
//builder.Services.AddScoped<ICheckoutRepository, CheckoutRepository>();

builder.Services.AddRabbitMq(builder.Configuration);
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();

// Configure EF Core with MySQL/MariaDB (adjust connection string and server version as needed)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<CheckoutSagaDbContext>(opts =>
    opts.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

namespace CheckoutService.Api
{
    public class Program
    {
    }
}