using consware_api.Application.Interfaces;
using consware_api.Application.Services;
using consware_api.Domain.Repositories;
using consware_api.Infrastructure.Data;
using consware_api.Infrastructure.Repositories;
using consware_api.Infrastructure.Services;
using consware_api.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

EnvironmentLoader.LoadFromFile();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Entity Framework
var dbConfig = DatabaseConfiguration.LoadFromEnvironment();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(dbConfig.GetConnectionString()));

// Add repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITravelRequestRepository, TravelRequestRepository>();

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITravelRequestService, TravelRequestService>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
