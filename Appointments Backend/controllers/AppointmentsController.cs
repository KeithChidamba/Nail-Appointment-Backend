using Microsoft.AspNetCore.Authorization;
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
    
    public AppointmentsController(AppointmentDbContext context)
    {
        _context = context;
    }
    [HttpGet("getConfirmed")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
    {
        List<Appointment> appointments = await _context.Appointments.ToListAsync();
        appointments.RemoveAll(a => DateTime.Parse(a.AppointmentDate) <= DateTime.Now.AddDays(-1));
        appointments.RemoveAll(a => a.isConfirmed==0);
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentTime).CompareTo(DateTime.Parse(b.AppointmentTime)));
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentDate).CompareTo(DateTime.Parse(b.AppointmentDate)));
        return appointments;
    }
    [Authorize]
    [HttpGet("getPending")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetPendingAppointments()
    {
        List<Appointment> appointments = await _context.Appointments.ToListAsync();
        appointments.RemoveAll(a => DateTime.Parse(a.AppointmentDate) <= DateTime.Now.AddDays(-1));
        appointments.RemoveAll(a => a.isConfirmed==1);
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentTime).CompareTo(DateTime.Parse(b.AppointmentTime)));
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentDate).CompareTo(DateTime.Parse(b.AppointmentDate)));
        return appointments;
    }
    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> CreateAppointment([FromBody] string appointment)
    {
        Appointment newAppointment =  JsonConvert.DeserializeObject<Appointment>(appointment);
        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync();
        return Ok(appointment);
    }
}