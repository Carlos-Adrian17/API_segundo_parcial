using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddCors(options => {
options.AddDefaultPolicy(policy => {
// Para desarrollo pueden usar AllowAnyOrigin()
// Para producción, especifiquen su URL de Azure: .WithOrigins("https://mi-sitio.azurewebsites.net")
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();

// 1. Extraer la cadena de conexión del appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar el DbContext con Pomelo MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString) // Esto detecta si es MySQL 8.0, 5.7, etc.
    )
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Accedes vía /scalar/v1

}

// Ensure CORS middleware runs early so responses (including error responses)
// include the appropriate CORS headers.
app.UseCors();

// Global exception handler: returns a simple JSON error and ensures CORS header is present
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        // In case CORS headers are not already applied, ensure the allowed origin is present
        if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var error = feature?.Error?.Message ?? "An unexpected error occurred.";
        var payload = JsonSerializer.Serialize(new { error });
        await context.Response.WriteAsync(payload);
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
