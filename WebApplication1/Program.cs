using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering",
    "Scorching"
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
    .WithName("GetWeatherForecast");


app.MapGet("/api/data/chart", async (HttpContext context) =>
    {
        context.Response.ContentType = "application/json";

        var today = DateTime.Today;
        var startOfMonth = new DateOnly(today.Year, today.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
        var random = new Random();

        await using var stream = context.Response.BodyWriter.AsStream();
        await using var writer = new Utf8JsonWriter(stream);

        writer.WriteStartArray();

        for (int i = 0; i < daysInMonth; i++)
        {
            var date = startOfMonth.AddDays(i);
            var value = Math.Round(random.NextDouble() * (0.010 - 0.002) + 0.002, 6);

            writer.WriteStartObject();
            writer.WriteString("date", date.ToString("yyyy-MM-dd"));
            writer.WriteNumber("value", value);
            writer.WriteEndObject();

            await writer.FlushAsync(); // Ghi t?ng ph?n ngay l?p t?c
        }

        writer.WriteEndArray();
        await writer.FlushAsync();
    })
    .WithName("GetChartData");
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
