using Microsoft.AspNetCore.Identity;
using SeniorBackend.Core;
using SeniorBackend.Infrastructure;
using SeniorBackend.Infrastructure.Models;
using SeniorBackend.WebApi.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApplicationLayer(); 
builder.Services.AddEndpointsApiExplorer(); 
 
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddApiVersioningExtension();
var app = builder.Build();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
