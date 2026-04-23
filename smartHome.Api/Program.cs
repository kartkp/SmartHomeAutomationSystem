using Microsoft.EntityFrameworkCore;
using smartHome.Api.Extensions;
using smartHome.Api.Middleware;
using smartHome.Infrastructure.Data;
using smartHome.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SmartHomeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAppServices();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();