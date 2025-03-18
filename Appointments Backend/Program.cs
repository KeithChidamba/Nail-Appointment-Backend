var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/GetAppointments", () =>
    {

    })
    .WithName("GetAppointments")
    .WithOpenApi();
app.MapPost("/NewAppointments", () =>
    {

    })
    .WithName("NewAppointments")
    .WithOpenApi();
app.Run();
