using Microsoft.EntityFrameworkCore;
using BookingService.Domain.Interfaces;
using BookingService.Infrastructure.Data;
using BookingService.Infrastructure.Repositories;
using BookingService.Application.Services;
using DotNetEnv;
using BookingService.Application.Clients;
using BookingService.Application.Services.Payment;

// Load variables from .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // nếu dùng cookie hoặc Authorization
        });
});

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

builder.Services.AddHttpClient<ShowtimeSeatClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7156"); // ShowtimeService
});

// Dependency Injection - Register all Repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
//builder.Services.AddScoped<IBookingFnbRepository, BookingFnbRepository>();
builder.Services.AddScoped<IBookingPromotionRepository, BookingPromotionRepository>();
builder.Services.AddScoped<IBookingSeatRepository, BookingSeatRepository>();
//builder.Services.AddScoped<IUsedPromotionRepository, UsedPromotionRepository>();
builder.Services.AddScoped<IPaymentService, MockPaymentService>();
builder.Services.AddHostedService<BookingExpirationService>();

// Dependency Injection - Register all Business Services
builder.Services.AddScoped<BookingBusiness>();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true)
    .AddEnvironmentVariables();

builder.Services.AddHttpClient<PricingClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:Pricing"]);
});

builder.Services.AddHttpClient<ZaloPayClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7093/"); // PaymentService sandbox
});


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

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
