using Microsoft.EntityFrameworkCore;
using PricingService.Domain.Interfaces;
using PricingService.Infrastructure.Data;
using PricingService.Infrastructure.Repositories;
using PricingService.Application.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using DotNetEnv;

// Load variables from .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

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

// Dependency Injection - Business services
builder.Services.AddScoped<SeatPriceBusiness>();
builder.Services.AddScoped<FnbItemBusiness>();
builder.Services.AddScoped<PromotionBusiness>();

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

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
