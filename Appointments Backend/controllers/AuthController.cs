using Appointments_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Appointments_Backend.controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Dummy user validation

            if (model.Username == "admin" && model.Password == "password")
            {
                var token = _jwtService.GenerateToken("1", "Admin");
                Console.WriteLine("logged in");
               
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
