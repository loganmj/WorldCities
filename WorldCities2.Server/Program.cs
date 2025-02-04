using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Serilog;
using Serilog.Events;
using WorldCities2.Server.Data;

// Use non-commercial version of EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var builder = WebApplication.CreateBuilder(args);

// Add configuration to read from user secrets
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
// Add JSON options so that indentation is supproted on all browsers.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
});

// Add ApplicationDbContext and MySQL Server support
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Serilog support
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.MySQL(
      connectionString: ctx.Configuration.GetConnectionString("DefaultConnection"),
      restrictedToMinimumLevel: LogEventLevel.Information,
      tableName: "LogEvents",
      storeTimestampInUtc: true)
    .WriteTo.Console();
});

var app = builder.Build();

// Enable logging of HTTP requests
app.UseSerilogRequestLogging();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
