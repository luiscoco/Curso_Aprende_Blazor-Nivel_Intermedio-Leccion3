var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7188") // Replace with your Blazor app's URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register services for static files and Blazor framework files
builder.Services.AddRazorComponents();

// Use Environment instead of HostEnvironment
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7142/") });

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add middleware to serve static files and Blazor WebAssembly
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles(); // Serve Blazor WebAssembly framework files
app.UseStaticFiles();          // Serve static files for the Blazor app

// Enable routing
app.UseRouting();

// Enable CORS
app.UseCors();  // Add this line to enable the CORS middleware

// Add fallback route for Blazor WebAssembly
app.MapFallbackToFile("index.html");

// Define your WeatherForecast API endpoint
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
