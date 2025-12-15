using PaymentService.Application.Services;
using PaymentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// DbContext

builder.Services.AddDbContext<PaymentDbContext>(opt =>
    opt.UseNpgsql(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// DI
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<PaymentBusiness>();
builder.Services.AddHttpClient();
// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
    var body = await reader.ReadToEndAsync();
    context.Request.Body.Position = 0;
    Console.WriteLine($"Incoming request body: {body}");
    await next();
});


// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
