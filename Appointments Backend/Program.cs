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
        ValidIssuer = jwtIssuer, 
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddScoped<JwtService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
        policy.WithOrigins("https://keithchidamba.github.io", "http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("AppointmentsDB");
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseMySQL(connectionString));

var app = builder.Build();




app.UseCors("AllowSpecific"); 


app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Options)
    {
        // Respond immediately with 200 OK for preflight requests
        context.Response.StatusCode = 200;
        context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization, Content-Type");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE");
        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        await context.Response.CompleteAsync(); // Finish the response immediately
        return;
    }
    await next();
});


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.Run();
