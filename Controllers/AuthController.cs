using DryvetrackTest.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DryvetrackTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public AuthController(IConfiguration configuration, DataContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            // Look for user by username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == login.Username);
            if (user == null || !VerifyPassword(login.Password, user.PasswordHash))
            {
                // Invalid username or password
                return Unauthorized();
            }

            // Generate JWT token if credentials are valid
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Include the UserId
                new Claim(JwtRegisteredClaimNames.Sub, login.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Convert stored password to byte array
            byte[] storedHashBytes = Convert.FromBase64String(storedPasswordHash);

            // Extract the salt (first 16 bytes)
            byte[] salt = new byte[16];
            Array.Copy(storedHashBytes, 0, salt, 0, 16);

            // Hash the entered password using the extracted salt
            byte[] enteredHash = KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            // Extract the stored hash (the next 32 bytes after the salt)
            byte[] storedHash = new byte[32];
            Array.Copy(storedHashBytes, 16, storedHash, 0, 32);

            // Compare the hashes
            return enteredHash.SequenceEqual(storedHash);
        }
    }


    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
