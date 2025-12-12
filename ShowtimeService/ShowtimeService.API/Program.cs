using Microsoft.EntityFrameworkCore;
using ShowtimeService.Domain.Interfaces;
using ShowtimeService.Infrastructure.Data;
using ShowtimeService.Infrastructure.Repositories;
using ShowtimeService.Application.Services;
using ShowtimeService.API.Hubs;
using ShowtimeService.API.HubNotifiers;
using ShowtimeService.Application.Interfaces;
using StackExchange.Redis;
using DotNetEnv;
using ShowtimeService.API.Controllers;
using System.Net;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ShowtimeDbContext>(opt =>
    opt.UseNpgsql(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Repositories
builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
builder.Services.AddScoped<ITheaterRepository, TheaterRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IShowtimeRepository, ShowtimeRepository>();
builder.Services.AddScoped<IShowtimeSeatRepository, ShowtimeSeatRepository>();

// Services
builder.Services.AddScoped<ProvinceService>();
builder.Services.AddScoped<TheaterService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<SeatService>();
builder.Services.AddScoped<ShowtimeBusiness>();
builder.Services.AddScoped<ShowtimeSeatService>();

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"])
);

// SignalR MUST BE BEFORE HUB NOTIFIER
builder.Services.AddSignalR();

// Hub notifier (Scoped only)
builder.Services.AddScoped<ISeatHubNotifier, SeatHubNotifier>();

// Lock + Redis subscriber
builder.Services.AddScoped<SeatService>();        // needs IConnectionMultiplexer

builder.Services.AddHostedService<RedisExpirationSubscriber>();

// -----------------------------
// MOVIE CLIENT (FIXED)
// -----------------------------
builder.Services.AddHttpClient<MovieClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:MovieService"]);
    client.DefaultRequestVersion = new Version(1, 1);
    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseRouting();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<SeatHub>("/seatHub");
});

app.Run();
