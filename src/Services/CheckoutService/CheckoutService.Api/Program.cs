using BugBucks.Shared.Logging.Extensions;
using CheckoutService.Application.Interfaces;
using CheckoutService.Application.Services;
using CheckoutService.Infrastructure.Data;
using CheckoutService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddAppLogging();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddScoped<ICheckoutService, CheckoutServiceImplementation>();
builder.Services.AddScoped<ICheckoutRepository, CheckoutRepository>();

// Configure EF Core with MySQL/MariaDB (adjust connection string and server version as needed)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CheckoutDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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

public partial class Program
{
}