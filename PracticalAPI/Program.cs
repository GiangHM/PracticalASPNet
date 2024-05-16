using PracticalAPI.DIKeyedServices;
using PracticalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
