using Microsoft.EntityFrameworkCore;
using Appointments_Backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Appointments_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddScoped<JwtService>();
builder.Services.AddCors(options =>
{
    // options.AddPolicy("AllowSpecific",
    //     policy => policy.WithOrigins("https://keithchidamba.github.io", "http://localhost:5102")
    //         .AllowAnyMethod()
    //         .AllowAnyHeader());
    options.AddPolicy("AllowAll",
       policy => policy.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("AppointmentsDB");
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseMySQL(connectionString) );

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

//app.UseCors("AllowSpecific");
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
