using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DryvetrackTest.Data;
using DryvetrackTest.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DryvetrackTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InsuranceController : ControllerBase
    {
        private readonly DataContext _context;

        public InsuranceController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Insurance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Insurance>>> GetInsurance()
        {
            return await _context.Insurance.ToListAsync();
        }

        [HttpGet("getInsuranceByVin/{vin}")]
        public async Task<ActionResult<Insurance>> GetInsuranceByVin(string vin)
        {
            // Find the car by VIN
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.VIN == vin);

            if (car == null)
            {
                return NotFound($"Car with VIN {vin} not found.");
            }

            // Find the insurance by the car's InsuranceId
            var insurance = await _context.Insurance.FirstOrDefaultAsync(i => i.InsuranceId == car.InsuranceId);

            if (insurance == null)
            {
                return NotFound($"No insurance found for the car with VIN {vin}.");
            }

            // Return the insurance details
            return Ok(insurance);
        }

        [HttpPut("updateInsuranceByVin/{vin}")]
        public async Task<ActionResult<Insurance>> UpdateInsuranceByVin(string vin, [FromBody] Insurance updatedInsurance)
        {
            // Find the car by VIN
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.VIN == vin);

            if (car == null)
            {
                return NotFound($"Car with VIN {vin} not found.");
            }

            // Find the insurance by the car's InsuranceId
            var insurance = await _context.Insurance.FirstOrDefaultAsync(i => i.InsuranceId == car.InsuranceId);

            if (insurance == null)
            {
                return NotFound($"No insurance found for the car with VIN {vin}.");
            }

            // Update the insurance details
            insurance.LicensePlate = updatedInsurance.LicensePlate;
            insurance.Provider = updatedInsurance.Provider;
            insurance.ExpiryDate = updatedInsurance.ExpiryDate;

            // Save the changes
            await _context.SaveChangesAsync();

            return Ok(insurance);
        }

        // GET: api/Insurance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Insurance>> GetInsurance(int id)
        {
            var insurance = await _context.Insurance.FindAsync(id);

            if (insurance == null)
            {
                return NotFound();
            }

            return insurance;
        }

        // POST: api/Insurance
        [HttpPost]
        public async Task<ActionResult<Insurance>> PostInsurance([FromBody] Insurance insurance)
        {
            _context.Insurance.Add(insurance);
            await _context.SaveChangesAsync();

            // Return the created insurance object and the route to access it
            return CreatedAtAction(nameof(GetInsurance), new { id = insurance.InsuranceId }, insurance);
        }

        // PUT: api/Insurance/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInsurance(int id, [FromBody] Insurance insurance)
        {
            if (id != insurance.InsuranceId)
            {
                return BadRequest();
            }

            _context.Entry(insurance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InsuranceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Insurance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInsurance(int id)
        {
            var insurance = await _context.Insurance.FindAsync(id);
            if (insurance == null)
            {
                return NotFound();
            }

            _context.Insurance.Remove(insurance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InsuranceExists(int id)
        {
            return _context.Insurance.Any(e => e.InsuranceId == id);
        }
    }
}