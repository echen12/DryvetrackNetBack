using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using DryvetrackTest.Data;
using DryvetrackTest.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DryvetrackTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterModel model)
        {
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                return BadRequest("Username already exists.");
            }

            // hash the password
            var passwordHash = HashPassword(model.Password);

            var user = new User
            {
                Name = model.Name,
                Username = model.Username,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { id = user.UserId }, user);
        }

        private string HashPassword(string password)
        {
            // generate a salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // hash the password using the salt
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            // combine the salt and hash into a single array
            byte[] hashBytes = new byte[16 + 32]; // 16 bytes for salt, 32 bytes for hash
            Array.Copy(salt, 0, hashBytes, 0, 16); // Copy the salt at the start
            Array.Copy(hash, 0, hashBytes, 16, 32); // Copy the hash after the salt

            // return the result as a Base64 string
            return Convert.ToBase64String(hashBytes);
        }
    }

    public class RegisterModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}