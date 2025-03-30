using Appointments_Backend.Data;
using Appointments_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Appointments_Backend.controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AppointmentDbContext _context;
        public AuthController(AppointmentDbContext context)
        {
            _context = context;
        }
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Dummy user validation
            List<BusinessOwner> OwnerFound =  _context.BusinessOwners.Where(a=>a.OwnerEmail==model.OwnerEmail
            & a.OwnerPassword==model.OwnerPassword
            ).ToList();
            if(OwnerFound.Count>0){
                var token = _jwtService.GenerateToken("1", "Owner");
                Console.WriteLine("logged in");
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] BusinessOwner newOwner)
        {
            // Dummy user validation
            List<BusinessOwner> OwnerFound = _context.BusinessOwners.Where(a=>a.OwnerEmail==newOwner.OwnerEmail
            ).ToList();
            if(OwnerFound.Count>0){
                Console.WriteLine("user exists");
                return StatusCode(403, "The user already exists");
            }
            _context.BusinessOwners.Add(newOwner);
            await _context.SaveChangesAsync();
            var token = _jwtService.GenerateToken("1", "Owner");
            Console.WriteLine("user added");
            return Ok(new { Token = token });              
        }
    }

    public class LoginModel
    {
        public string BusinessName { get; set; }
        public string OwnerPassword { get; set; }
        public string OwnerEmail { get; set; }
    }
}
