using System.Globalization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Appointments_Backend.Data;
namespace Appointments_Backend.controllers;

[Route("api/appointments")]
[ApiController]
public class AppointmentsController : Controller
{
    private readonly AppointmentDbContext _context;
    private readonly ILogger<AppointmentsController> _logger;
    
    public AppointmentsController(AppointmentDbContext context,ILogger<AppointmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/users
    [HttpGet("getAll")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
    {
        List<Appointment> appointments = await _context.Appointments.ToListAsync();
        appointments.RemoveAll(a => DateTime.Parse(a.AppointmentDate) <= DateTime.Now.AddDays(-1));
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentTime).CompareTo(DateTime.Parse(b.AppointmentTime)));
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentDate).CompareTo(DateTime.Parse(b.AppointmentDate)));
        return appointments;
    }
    // POST: api/users
    [HttpPost("add")]
    public async Task<IActionResult> CreateAppointment([FromBody] string appointment)
    {
        _logger.LogInformation($"Received: {appointment}");
        Appointment newAppointment =  JsonConvert.DeserializeObject<Appointment>(appointment);
        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync();
        return Ok(appointment);
    }
}