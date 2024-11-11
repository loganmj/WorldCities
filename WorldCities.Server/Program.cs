using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add configuration to read from user secrets
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
// Add JSON options so that indentation is supproted on all browsers.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add ApplicationDbContext and MySQL Server support
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(connectionString ?? throw new InvalidOperationException("Connection string is missing or empty.")));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
