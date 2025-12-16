using Azure.Messaging.ServiceBus;
using Cart.API.Services;
using Cart.BLL.Middleware;
using LiteDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Authorization for both roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BothRoles", policy => policy.RequireRole("Manager", "StoreCustomer"));
});

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

app.UseMiddleware<TokenLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
