using Microsoft.EntityFrameworkCore;
using BookingService.Domain.Interfaces;
using BookingService.Infrastructure.Data;
using BookingService.Infrastructure.Repositories;
using BookingService.Application.Services;
using DotNetEnv;

// Load variables from .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DB context (Neon PostgreSQL)
builder.Services.AddDbContext<BookingDbContext>(opt =>
    opt.UseNpgsql(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Dependency Injection - Register all Repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingFnbRepository, BookingFnbRepository>();
builder.Services.AddScoped<IBookingPromotionRepository, BookingPromotionRepository>();
builder.Services.AddScoped<IBookingSeatRepository, BookingSeatRepository>();
builder.Services.AddScoped<IUsedPromotionRepository, UsedPromotionRepository>();

// Dependency Injection - Register all Business Services
builder.Services.AddScoped<BookingBusiness>();
builder.Services.AddScoped<BookingFnbBusiness>();
builder.Services.AddScoped<BookingPromotionBusiness>();
builder.Services.AddScoped<BookingSeatBusiness>();
builder.Services.AddScoped<UsedPromotionBusiness>();

var app = builder.Build();

// Auto apply migration on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db.Database.Migrate();

    // Nếu bạn có DbInitializer thì dùng, chưa có thì bỏ dòng sau.
    // DbInitializer.Initialize(db);
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
