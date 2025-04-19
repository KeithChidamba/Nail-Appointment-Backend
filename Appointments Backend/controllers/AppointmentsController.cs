using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Appointments_Backend.Data;
using Microsoft.AspNetCore.Authorization;
namespace Appointments_Backend.controllers;

[Route("api/appointments")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentDbContext _context;
    
    public AppointmentsController(AppointmentDbContext context)
    {
        _context = context;
    }
    [HttpGet("GetForClientView/{BusinessName}")]
    public async Task<ActionResult<string>> GetAppointments(string BusinessName)
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
            return JsonConvert.SerializeObject(appointments);
        }
        return StatusCode(404, "This business does'nt exist");
    }
    [Authorize]
    [HttpGet("GetForBusinessView")]
    public async Task<ActionResult<string>> GetPendingAppointments()
    {
        var BusinessID = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        List<Appointment> appointments = await _context.Appointments.ToListAsync();
        appointments.RemoveAll(a => DateTime.Parse(a.AppointmentDate) <= DateTime.Now.AddDays(-1));
        appointments.RemoveAll(a => a.BusinessID.ToString() != BusinessID);
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentTime).CompareTo(DateTime.Parse(b.AppointmentTime)));
        appointments.Sort((a, b) => DateTime.Parse(a.AppointmentDate).CompareTo(DateTime.Parse(b.AppointmentDate)));
        return JsonConvert.SerializeObject(appointments);
    }
    [Authorize]
    [HttpPost("update")]
    public async Task<IActionResult> UpdateAppointment([FromBody] Appointment UpdatedData)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var currentAppointment = _context.Appointments.FirstOrDefault(a => a.AppointmentID == UpdatedData.AppointmentID);
        if (currentAppointment == null) return NotFound("Appointment not found");

        _context.Entry(currentAppointment).CurrentValues.SetValues(UpdatedData);
        await _context.SaveChangesAsync();
        return Ok("updated appointment");
    }

    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> CreateAppointment([FromBody] Appointment newAppointment)
    {
        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync();
        return Ok("added appointment");
    }
}