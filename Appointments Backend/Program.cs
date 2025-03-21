using Microsoft.EntityFrameworkCore;
using Appointments_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific",
        policy => policy.WithOrigins("https://keithchidamba.github.io")
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("AppointmentsDB");
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseMySQL(connectionString) );

var app = builder.Build();

app.UseCors("AllowSpecific");

app.MapControllers();

app.Run();
