using Microsoft.EntityFrameworkCore;
using MovieService.Domain.Interfaces;
using MovieService.Infrastructure.Data;
using MovieService.Infrastructure.Repositories;
using MovieService.Application.Services;
using DotNetEnv; 

// Load variables from .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DB context (Neon PostgreSQL)
builder.Services.AddDbContext<MovieDbContext>(opt =>
	opt.UseNpgsql(
		Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
		?? builder.Configuration.GetConnectionString("DefaultConnection")
	));

// Dependency Injection
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<MovieBusiness>();

var app = builder.Build();

// Auto apply migration on startup
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
	db.Database.Migrate();
	DbInitializer.Initialize(db); // Seed sample database
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
