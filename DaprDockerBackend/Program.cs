using Dapr;
using DaprDockerBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddDapr();

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

app.UseRouting();
app.UseCloudEvents();

app.UseAuthorization();

app.MapControllers();

//app.MapPost("/orders", [Topic("servicebus-pubsub", "orders")] (Basket basket) =>
//{
//    Console.WriteLine($"Received a new order from {basket.Id}");

//    return Results.Ok();
//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
    endpoints.MapControllers();
});

app.Run();
