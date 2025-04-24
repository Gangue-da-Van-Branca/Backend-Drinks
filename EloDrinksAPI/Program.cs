using EloDrinksAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Lê a connection string
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

Console.WriteLine("CONN: " + Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"));

// Adiciona o DbContext com MySQL
builder.Services.AddDbContext<ElodrinkContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
