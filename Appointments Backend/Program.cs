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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,  // The issuer of the token
        ValidAudience = jwtIssuer,  // The expected audience of the token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Optional: strict expiry validation
    };
});
builder.Services.AddScoped<JwtService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific",
        policy => policy.WithOrigins("https://keithchidamba.github.io", "http://localhost:5102", "http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());  // Allow credentials if needed
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("AppointmentsDB");
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseMySQL(connectionString));

var app = builder.Build();

// CORS must be used before authentication
app.UseCors("AllowSpecific"); // Apply CORS policy here

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
