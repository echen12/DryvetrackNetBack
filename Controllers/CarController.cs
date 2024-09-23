using DryvetrackTest.Data;
using DryvetrackTest.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly DataContext _context;

        public class UpdateMileageRequest
        {
            public int NewMileage { get; set; }
        }

        public CarController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("addCar")]
        public async Task<ActionResult<Car>> AddCar()
        {
            // Read the request body
            using var reader = new StreamReader(HttpContext.Request.Body);
            var body = await reader.ReadToEndAsync();

            // Deserialize the JSON into the Car object
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

            // Create the insurance object from the general object
            var insurance = new Insurance
            {
                LicensePlate = generalObject.GetProperty("plateNumber").GetString(),
                Provider = generalObject.GetProperty("insuranceProvider").GetString(),
                ExpiryDate = generalObject.GetProperty("insuranceExpiryDate").GetDateTime()
            };

            // Save the insurance object first
            _context.Insurance.Add(insurance);
            await _context.SaveChangesAsync(); // Save to get the generated InsuranceId

            // Now create the car object and associate it with the insurance
            var car = new Car
            {
                VIN = generalObject.GetProperty("vin").GetString(),
                Make = generalObject.GetProperty("make").GetString(),
                Model = generalObject.GetProperty("model").GetString(),
                Color = generalObject.GetProperty("color").GetString(),
                VehicleType = generalObject.GetProperty("vehicleType").GetString(),
                ModelYear = generalObject.GetProperty("modelYear").GetString(),
                CurrentMileage = int.Parse(generalObject.GetProperty("mileage").GetString()),
                UserId = userId,
                InsuranceId = insurance.InsuranceId // Associate the insurance
            };

            // Save the car object
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarByVin), new { vin = car.VIN }, new { car, insurance });


        }

        [HttpGet("mycars")]
        public async Task<ActionResult<IEnumerable<Car>>> GetMyCars()
        {
            // Get the user ID from the JWT token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Fetch cars for the authenticated user
            var cars = await _context.Cars.Where(c => c.UserId == userId).ToListAsync();

            return Ok(cars);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var cars = await _context.Cars.ToListAsync();
            return Ok(cars);
        }

        //[HttpPost]
        //public async Task<ActionResult<Car>> AddCarTest(Car car)
        //{
        //    if (car == null)
        //    {
        //        return BadRequest("Car object is null.");
        //    }

        //    // Optionally, you can validate the car data here

        //    _context.Cars.Add(car);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetCars), new { id = car.Id }, car);
        //}

        [HttpGet("{vin}")]
        public async Task<ActionResult<Car>> GetCarByVin(string vin)
        {
            var car = await _context.Cars.SingleOrDefaultAsync(c => c.VIN == vin);

            if (car == null)
            {
                return NotFound($"Car with VIN {vin} not found.");
            }

            return Ok(car);
        }

        [HttpPut("updateMileage/{vin}")]
        public async Task<ActionResult<Car>> UpdateMileage(string vin, [FromBody] UpdateMileageRequest request)
        {
            // Validate input
            if (request.NewMileage < 0)
            {
                return BadRequest("Mileage must be a positive value.");
            }

            // Find the car by VIN
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.VIN == vin);
            if (car == null)
            {
                return NotFound($"Car with VIN {vin} not found.");
            }

            // Update the mileage
            car.CurrentMileage = request.NewMileage;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(car);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteCar(string id)
        {
            // Find the car by ID
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound($"Car with ID {id} not found.");
            }

            // Remove the associated insurance if it exists
            if (car.InsuranceId != null)
            {
                var insurance = await _context.Insurance.FindAsync(car.InsuranceId);
                if (insurance == null)
                {
                    return NotFound($"Insurance with ID {insurance.InsuranceId} not found.");
                }

                // Remove the insurance from the context
                _context.Insurance.Remove(insurance);
                await _context.SaveChangesAsync(); // Save changes to the database
            }

            // Remove the car from the context
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync(); // Save changes to the database

            return NoContent(); // Return 204 No Content status
        }


    }
}
