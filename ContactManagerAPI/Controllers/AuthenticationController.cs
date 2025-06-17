using ContactManagerApi.Models;
using ContactManagerApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContactManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ContactContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ContactContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(User request)
        {
            if (_context.Users.Any(u => u.Username == request.Username))
                return BadRequest("User already exists");

            _context.Users.Add(request);
            _context.SaveChanges();
            return Ok("User created successfully");
        }

        [HttpPost("login")]
        public IActionResult Login(User request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
            if (user == null)
                return BadRequest("Invalid credentials");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
