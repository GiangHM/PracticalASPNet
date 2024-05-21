using Microsoft.AspNetCore.RateLimiting;
using PracticalAPI.CustomMiddleware;
using PracticalAPI.DIKeyedServices;
using PracticalAPI.RateLimitMiddleware.Extensions;
using PracticalAPI.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Read from configuration
builder.Services.AddAppRateLimit(option =>
{
    option.PermitLimit = 9;
    option.QueueLimit = 10;
});

// Add services to the container.
builder.Services.AddKeyedTransient<IGreeting, FormalGreeting>("FormalGreeting");
builder.Services.AddKeyedTransient<IGreeting, InformalGreeting>("InformalGreeting");
builder.Services.AddTransient<IWelcoming, Welcoming>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Step 3: Use our custom middle in app
app.UseCustomExceptionHandlers();

app.UseHttpsRedirection();

app.UseRouting();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
