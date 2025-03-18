using Microsoft.EntityFrameworkCore;
using Appointments_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("AppointmentsDB");
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseMySQL(connectionString) );

var app = builder.Build();

app.UseCors("AllowAll"); 

app.MapControllers();

app.Run();
