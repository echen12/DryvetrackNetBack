using DryvetrackTest.Data;
using DryvetrackTest.Entities;
using DryvetrackTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace DryvetrackTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly CarService _carService;
        private readonly DataContext _context;

        public CarController(CarService carService, DataContext context)
        {
            _carService = carService;
            _context = context;
        }

        [HttpPost("addCar")]
        public async Task<ActionResult<Car>> AddCar()
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
            var body = await reader.ReadToEndAsync();
            dynamic generalObject = JsonSerializer.Deserialize<dynamic>(body);

            if (generalObject.ValueKind == JsonValueKind.Null)
            {
                return BadRequest("Invalid car data.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User is not authenticated.");
            }

            int userId = int.Parse(userIdClaim);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var car = await _carService.AddCar(user, generalObject);
            return CreatedAtAction(nameof(GetCarByVin), new { vin = car.VIN }, car);
        }

        [HttpGet("mycars")]
        public async Task<ActionResult<IEnumerable<Car>>> GetMyCars()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var cars = await _carService.GetCarsByUserId(userId);
            return Ok(cars);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var cars = await _context.Cars.ToListAsync();
            return Ok(cars);
        }

        [HttpGet("{vin}")]
        public async Task<ActionResult<Car>> GetCarByVin(string vin)
        {
            var car = await _carService.GetCarByVin(vin);
            if (car == null)
            {
                return NotFound($"Car with VIN {vin} not found.");
            }

            return Ok(car);
        }

        [HttpPut("updateMileage/{vin}")]
        public async Task<ActionResult> UpdateMileage(string vin, [FromBody] UpdateMileageRequest request)
        {
            if (request.NewMileage < 0)
            {
                return BadRequest("Mileage must be a positive value.");
            }

            await _carService.UpdateMileage(vin, request.NewMileage);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteCar(string id)
        {
            await _carService.DeleteCar(id);
            return NoContent();
        }
    }

    public class UpdateMileageRequest
    {
        public int NewMileage { get; set; }
    }
}