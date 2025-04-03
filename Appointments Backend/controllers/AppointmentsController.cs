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
    [HttpGet("getConfirmed/{BusinessName}")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments(string BusinessName)
    {
        if (string.IsNullOrEmpty(BusinessName))
        {
            return BadRequest("Business name is required.");
        }
        List<Business> OwnerFound = _context.BusinessOwners.Where(a=>a.BusinessName==BusinessName).ToList();
        if(OwnerFound.Count>0){
            List<Appointment> appointments = await _context.Appointments.ToListAsync();
            appointments.RemoveAll(a => DateTime.Parse(a.AppointmentDate) <= DateTime.Now.AddDays(-1));
            appointments.RemoveAll(a => a.isConfirmed==0);
            appointments.RemoveAll(a => a.BusinessID!=OwnerFound[0].BusinessID);
            appointments.Sort((a, b) => DateTime.Parse(a.AppointmentTime).CompareTo(DateTime.Parse(b.AppointmentTime)));
            appointments.Sort((a, b) => DateTime.Parse(a.AppointmentDate).CompareTo(DateTime.Parse(b.AppointmentDate)));
            return appointments;
        }
        return StatusCode(404, "This business is does'nt exist");
    }
    [Authorize]
    [HttpGet("getPending/{BusinessID}")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetPendingAppointments(int BusinessID)
    {
        List<Appointment> appointments = await _context.Appointments.ToListAsync();
        appointments.RemoveAll(a => DateTime.Parse(a.AppointmentDate) <= DateTime.Now.AddDays(-1));
        appointments.RemoveAll(a => a.BusinessID!=BusinessID);
        appointments.RemoveAll(a => a.isConfirmed==1);
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentTime).CompareTo(DateTime.Parse(b.AppointmentTime)));
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentDate).CompareTo(DateTime.Parse(b.AppointmentDate)));
        return appointments;
    }
    [HttpPost("add")]
    public async Task<IActionResult> CreateAppointment([FromBody] string appointment)
    {
        Appointment newAppointment =  JsonConvert.DeserializeObject<Appointment>(appointment);
        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync();
        return Ok(appointment);
    }
}