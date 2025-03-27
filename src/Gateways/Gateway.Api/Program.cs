var builder = WebApplication.CreateBuilder(args);

// Add YARP reverse proxy services and load configuration from appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseRouting();

// Map the reverse proxy middleware
app.UseEndpoints(endpoints => { endpoints.MapReverseProxy(); });

app.Run();