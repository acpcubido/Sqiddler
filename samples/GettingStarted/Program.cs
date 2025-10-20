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
app.MapGet("/weatherforecast/{id}", (SqidParam<WeatherForecast> id) =>
{
    var forecast = GetWeatherForecast(id);
    return forecast;
})
.WithName("GetWeatherForecastDay");
app.MapGet("/weatherforecast/multiple/{ids}", (SqidArrayParam<WeatherForecast> ids) =>
{
    var forecasts = ids.Value.Select(GetWeatherForecast).ToArray();
    return forecasts;
})
.WithName("GetWeatherForecastMultiple");
app.MapGet("/weatherforecast/batch", () =>
{
    var ids = new[] { 1, 2, 3 };
    var batch = new WeatherForecastBatch
    {
        Ids = ids,
        Forecasts = ids.Select(GetWeatherForecast).ToArray()
    };
    return batch;
})
.WithName("GetWeatherForecastBatch");

app.Run();

internal class WeatherForecast()
{
    [JsonSqid<WeatherForecast>]
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal class WeatherForecastBatch()
{
    [JsonSqid<WeatherForecastBatch>]
    public required int[] Ids { get; set; }

    public required WeatherForecast[] Forecasts { get; set; }
}
