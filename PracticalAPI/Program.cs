using Microsoft.AspNetCore.RateLimiting;
using PracticalAPI.CustomMiddleware;
using PracticalAPI.DIKeyedServices;
using PracticalAPI.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Api", options =>
    {
        options.AutoReplenishment = true;
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromMinutes(1);
    });

    options.AddFixedWindowLimiter("Web", options =>
    {
        options.AutoReplenishment = true;
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromMinutes(1);
    });
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

app.UseAuthorization();

app.MapControllers();

app.Run();
