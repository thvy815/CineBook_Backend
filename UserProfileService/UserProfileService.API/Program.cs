using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using UserProfileService.Application.Services;
using UserProfileService.Domain.Interfaces;
using UserProfileService.Infrastructure.Repositories;
using UserProfileService.Infrastructure.Data;

// Load variables from .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DB context (Neon PostgreSQL)
builder.Services.AddDbContext<UserProfileDbContext>(opt =>
	opt.UseNpgsql(
		Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
		?? builder.Configuration.GetConnectionString("DefaultConnection")
	));

// Dependency Injection
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<UserProfileBusiness>();

var app = builder.Build();

// Auto apply migration on startup
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<UserProfileDbContext>();
	db.Database.Migrate(); // Update database to latest migration
	await DbInitializer.SeedAsync(db); // Seed sample data
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
