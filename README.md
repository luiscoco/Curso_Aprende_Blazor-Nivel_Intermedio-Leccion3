# CURSO: APRENDE BLAZOR (NIVEL INTERMEDIO)

# LECCIÓN 3: Crear la arquitectura básica de la aplicación Blazing Pizza para .NET8 y .NET9

1. Ejecutar Visual Studio 2022.

2. Crear un proyecto de WebAssebly con la siguiente línea de comandos en dotNet CLI:

```
dotnet new blazorwasm -o BlazingPizzaWebAssemply
```

3. Crear una aplicación de tipo ASP.NET Core Web API:

```
dotnet new webapi -o BlazingPizzaWebAPI
```

4. Crear la Solución que hospedará los dos proyectos anteriores:

```
dotnet new sln -o BlazingPizza
```

5. Ejecutar Visual Studio 2022 Community Edition y añadir los proyectos creados a la solución

6. En el proyecto Web API Project añadir la referencia al proyecto WebAssembly

7. En el proyecto Web API Project añadir la siguiente biblioteca con Nuget: 

```
Microsoft.AspNetCore.Components.WebAssembly.Server
```

8. Incluir la dirección de la aplicación Web API para invocarla desde el componente weatherforecast de la aplicación Cliente WebAssembly:

Pages/Weather.razor

```razor
@page "/weather"
@inject HttpClient Http

<PageTitle>Weather</PageTitle>
<h1>Weather</h1>
<p>This component demonstrates fetching data from the server.</p>

@if (forecasts == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var forecast in forecasts)
            {
                <tr>
                    <td>@forecast.Date.ToShortDateString()</td>
                    <td>@forecast.TemperatureC</td>
                    <td>@forecast.TemperatureF</td>
                    <td>@forecast.Summary</td>
                </tr>
            }
        </tbody>
    </table>
}
@code {
    private WeatherForecast[]? forecasts;
    protected override async Task OnInitializedAsync()
    {
        forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7142/weatherforecast");
    }
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }ç
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
```

9. Modificar el Middleware(archivo Program.cs) en el proyecto Web API Project.
   Incluimos el puerto de la aplicación WebAssembly para configurar el CORS
   Incluimos el puerto de la aplicación Web API para configurar el Servicio HttpCLient

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS support
builder.Services.AddCors(options =
{
    options.AddDefaultPolicy(policy =
    {
        policy.WithOrigins("https://localhost:7181") // Replace with your Blazor app's URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register services for static files and Blazor framework files
builder.Services.AddRazorComponents();

// Use Environment instead of HostEnvironment
builder.Services.AddScoped(sp = new HttpClient { BaseAddress = new Uri("https://localhost:7268/") });

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

app.MapGet("/weatherforecast", () =
{
    var forecast = Enumerable.Range(1, 5).Select(index =
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
    public int TemperatureF = 32 + (int)(TemperatureC / 0.5556);
}
```

10. Establecer ambos proyectos como proyectos de inicio. Ejecutar la aplicación.

11. (OPCIONAL) Añadir un nuevo proyecto a la solución de tipo "C# Class Library" para definir los Modelos de datos de la aplicación

12. (OPCIONAL) Añadir un nuevo proyecto a la solución de tipo "Razor Library" para hospedar el componente Mapa y la librería Leaflet para gestionar el mapa
