using Sqiddler;
using Sqiddler.AspNetCore;
using Sqiddler.OpenApi;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddSqids();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
WeatherForecast GetWeatherForecast(int index) => new()
{
    Id = index,
    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
    TemperatureC = Random.Shared.Next(-20, 55),
    Summary = summaries[Random.Shared.Next(summaries.Length)]
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(GetWeatherForecast).ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.MapGet("/weatherforecast/{id}", (
    // Demonstrates usage of SqidParam attribute usage on plain integer request parameters
    SqidParam<WeatherForecast> id) =>
{
    var forecast = GetWeatherForecast(id);
    return forecast;
})
.WithName("GetWeatherForecastDay");

app.MapGet("/weatherforecast/bulk", (
    // Demonstrates usage of SqidParam attribute usage on query params
    SqidParam<WeatherForecast>[] ids,
    // Use this property to compare with Open API JSON output of ids
    string[] ids1) =>
{
    var forecasts = ids.Select(id => GetWeatherForecast((int)id)).ToArray();
    return forecasts;
})
.WithName("GetWeatherForecastBulk");

app.Run();

internal class WeatherForecast()
{
    /// <summary>
    /// Demonstrates usage of JsonSqid attribute usage on plain integer properties
    /// </summary>
    [JsonSqid<WeatherForecast>]
    public int Id { get; set; }

    /// <summary>
    /// Demonstrates usage of JsonSqid attribute usage on arrays
    /// </summary>
    [JsonSqid<WeatherForecast>]
    public int[] Ids { get; set; } = null!;

    /// <summary>Use this property to compare with Open API JSON output of <see cref="Id"/></summary>
    public string IdReference { get; set; } = null!;

    /// <summary>Use this property to compare with Open API JSON output of <see cref="Ids"/></summary>
    public string[] IdsReference { get; set; } = null!;

    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
