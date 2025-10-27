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

var tmdbApiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY")
                 ?? builder.Configuration["TMDB_API_KEY"];
if (string.IsNullOrEmpty(tmdbApiKey))
    throw new InvalidOperationException("TMDB_API_KEY not found in environment or appsettings.json");


builder.Services.AddHttpClient<TmdbService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "CineBookMovieService");
});


// Dependency Injection
builder.Services.AddScoped<IMovieDetailRepository, MovieDetailRepository>();
builder.Services.AddScoped<MovieDetailService>();
builder.Services.AddHttpClient<TmdbService>();

// Add API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Services.GetRequiredService<IConfiguration>()["TMDB_API_KEY"] = tmdbApiKey;

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
