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
        public AuthController(AppointmentDbContext context,JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        private readonly JwtService _jwtService;
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Dummy user validation
            List<Business> OwnerFound =  _context.BusinessOwners.Where(a=>a.OwnerEmail==model.OwnerEmail
            & a.OwnerPassword==model.OwnerPassword
            ).ToList();
            if(OwnerFound.Count>0){
                var token = _jwtService.GenerateToken("1", "Owner");
                Console.WriteLine("logged in");
                return Ok(new { Token = token });
            }
            return StatusCode(404, "This business is does'nt exist");
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Business newOwner)
        {
            List<Business> OwnerFound = _context.BusinessOwners.Where(a=>a.OwnerEmail==newOwner.OwnerEmail
            || a.BusinessName==newOwner.BusinessName).ToList();
            if(OwnerFound.Count>0){
                Console.WriteLine("user exists");
                return StatusCode(403, "This business is already registered");
            }
            _context.BusinessOwners.Add(newOwner);
            await _context.SaveChangesAsync();
            var token = _jwtService.GenerateToken("1", "Owner");
            Console.WriteLine("user added");
            return Ok(new { Token = token });              
        }
        [HttpGet("GetBusiness")]
        public async Task<ActionResult<Business>> GetBusinessData([FromBody] LoginModel owner)
        {
            List<Business> OwnerFound =  _context.BusinessOwners.Where(a=>a.OwnerEmail==owner.OwnerEmail
            & a.OwnerPassword==owner.OwnerPassword
            ).ToList();
            if(OwnerFound.Count>0){

               return OwnerFound.First();
            }
            return StatusCode(404, "This business is does'nt exist");
        }
    }

    public class LoginModel
    {
        public string BusinessName { get; set; }
        public string OwnerPassword { get; set; }
        public string OwnerEmail { get; set; }
    }
}
