using Microsoft.EntityFrameworkCore;
using BookingService.Infrastructure.Data;
using BookingService.Infrastructure.Repositories;
using BookingService.Domain.Interfaces;
using BookingService.Application.Services;
using BookingService.Application.Clients;
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
builder.Services.AddDbContext<BookingDbContext>(opt =>
    opt.UseNpgsql(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ));


builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<BookingBusiness>();
builder.Services.AddScoped<PricingClient>();

builder.Services.AddHttpClient<ShowtimeSeatClient>(c =>
{
    c.BaseAddress = new Uri("https://localhost:7156"); // showtime-service
});

var app = builder.Build();
app.MapControllers();
app.Run();
