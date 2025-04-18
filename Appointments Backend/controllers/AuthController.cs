using System.Security.Claims;
using Appointments_Backend.Data;
using Appointments_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
namespace Appointments_Backend.controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppointmentDbContext _context;
        public AuthController(AppointmentDbContext context,JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        private readonly JwtService _jwtService;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            List<Business> OwnerFound =  _context.BusinessOwners.Where(a=>a.BusinessName==model.BusinessName
            & a.OwnerPassword==model.OwnerPassword & a.OwnerEmail==model.OwnerEmail
            ).ToList();
            if(OwnerFound.Count>0){
                var token = _jwtService.GenerateToken(OwnerFound[0].BusinessID.ToString());
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid credentials.");
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Business newOwner)
        {
            List<Business> OwnerFound = _context.BusinessOwners.Where(a=>a.OwnerEmail==newOwner.OwnerEmail
            || a.BusinessName==newOwner.BusinessName).ToList();
            if(OwnerFound.Count>0){
                return StatusCode(403, "This business is already registered");
            }
            _context.BusinessOwners.Add(newOwner);
            await _context.SaveChangesAsync();
            var token = _jwtService.GenerateToken(newOwner.BusinessID.ToString());
            return Ok(new { Token = token });              
        }
    [Authorize]
    [HttpGet("GetBusinessData")]
    public async Task<ActionResult<Business>> GetBusinessData()
    {
        var BusinessID = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        var ownerFound = _context.BusinessOwners
            .Where(a => a.BusinessID.ToString() == BusinessID)
            .ToList();
        if (ownerFound.Count > 0)
        {
            return Ok(JsonConvert.SerializeObject(ownerFound.First()));
        }
        return NotFound("This business doesn't exist: "+BusinessID);
    }
    }

    public class LoginModel
    {
        public string BusinessName { get; set; }
        public string OwnerPassword { get; set; }
        public string OwnerEmail { get; set; }
    }
}
