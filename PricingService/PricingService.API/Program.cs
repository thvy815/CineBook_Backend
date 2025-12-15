using Microsoft.EntityFrameworkCore;
using PricingService.Domain.Interfaces;
using PricingService.Infrastructure.Repositories;
using DotNetEnv;

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

// Add DB context (PostgreSQL - e.g., Neon)
builder.Services.AddDbContext<PricingDbContext>(opt =>
    opt.UseNpgsql(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Dependency Injection - Repositories
builder.Services.AddScoped<ISeatPriceRepository, SeatPriceRepository>();
builder.Services.AddScoped<IFnbItemRepository, FnbItemRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();

builder.Services.AddScoped<PricingService.Infrastructure.Repositories.PricingRepository>();
builder.Services.AddScoped<PricingService.Application.Services.PricingBusiness>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto apply migration on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PricingDbContext>();
    db.Database.Migrate();

    // Nếu bạn có lớp DbInitializer cho Pricing thì gọi:
    // DbInitializer.Initialize(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
