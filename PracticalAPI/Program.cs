using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using PracticalAPI.AppConfiguration;
using PracticalAPI.AuthorizationRequirementData;
using PracticalAPI.CustomMiddleware;
using PracticalAPI.DIKeyedServices;
using PracticalAPI.ExceptionHandlers;
using PracticalAPI.RateLimitMiddleware.Extensions;
using PracticalAPI.RouterwareSamples;
using PracticalAPI.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// AppConfiguration section
// With Sentinel key for refreshing configs
//builder.Configuration.AddAppConfigurationWithSentinelKey();

// Use Azure Key Vault
//builder.Configuration.AddAzureKeyVault(
//    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
//    new DefaultAzureCredential(new DefaultAzureCredentialOptions
//    {
//        ManagedIdentityClientId = builder.Configuration["AzureADManagedIdentityClientId"]
//    }));

// Apply Rate limit
// I want to apply it to Link Receiver
//builder.Services.AddAppRateLimit(option =>
//{
//    option.PermitLimit = 9;
//    option.QueueLimit = 10;
//});

// Add services to the container.
builder.Services.AddKeyedTransient<IGreeting, FormalGreeting>("FormalGreeting");
builder.Services.AddKeyedTransient<IGreeting, InformalGreeting>("InformalGreeting");
builder.Services.AddTransient<IWelcoming, Welcoming>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// I want to use request decompression to integrate with WMT 
// Side note: => should be finish on AF HttpTrigger
//builder.Services.AddRequestDecompression();


// User Exception Handler
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Use Custom Authorization
// builder.Services.AddSingleton<IAuthorizationHandler, UserFeatureAuthorizationHandler>();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Step 3: Use our custom middle in app
//app.UseCustomExceptionHandlers();

app.UseHttpsRedirection();

// I want to use request decompression to integrate with WMT 
// Side note: => should be finish on AF HttpTrigger
//app.UseRequestDecompression();

// With Sentinel key for refreshing configs
//app.UseAzureAppConfiguration();

app.UseRouting();

//Use Authentication and Authorization
// app.UseAuthentication();
// app.UseAuthorization();

//app.UseRateLimiter();

//app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapAuthor("/author");
});

app.Run();
