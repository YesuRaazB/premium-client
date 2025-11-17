using premium.Api.Data;
using premium.Api.Services;
using Microsoft.EntityFrameworkCore;

//internal class Program
//{
//    private static void Main(string[] args)
//    {
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQL Server
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<premiumDbContext>(options =>
    options.UseSqlServer(conn));


//builder.Services.AddDbContext<InsuranceDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IPremiumCalculator, PremiumCalculator>();

// CORS - allow React dev server origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevAllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});




var app = builder.Build();

// Apply migrations automatically (dev convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<premiumDbContext>();
    db.Database.Migrate();
}

app.UseCors("DevAllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
//    }
//}
// Required for WebApplicationFactory<Program> used by integration tests:
public partial class Program { }







//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast =  Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

//app.Run();

//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
