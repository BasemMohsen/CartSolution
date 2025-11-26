using Microsoft.AspNetCore.Mvc;
using LiteDB;
using Azure.Messaging.ServiceBus;
using Cart.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Cart.API.xml"));
});

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

// Dependency Injection for CartService
builder.Services.AddScoped<Cart.BLL.Interfaces.ICartService, Cart.BLL.Services.CartService>();
builder.Services.AddScoped<Company.Cart.DAL.Interfaces.ICartRepository, Company.Cart.DAL.LiteDb.LiteDbCartRepository>();

// Register LiteDB instance
builder.Services.AddSingleton<ILiteDatabase>(_ => new LiteDatabase("Filename=CartDatabase.db;Mode=Shared"));

// Register ServiceBusClient
builder.Services.AddSingleton<ServiceBusClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration["ServiceBus:ConnectionString"];
    return new ServiceBusClient(connectionString);
});

// Register ServiceBusListener as a hosted service
builder.Services.AddHostedService<ServiceBusListener>();

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
