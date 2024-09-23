using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DryvetrackTest.Entities;
using DryvetrackTest.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DryvetrackTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InsuranceController : ControllerBase
    {
        private readonly InsuranceService _insuranceService;

        public InsuranceController(InsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Insurance>>> GetInsurance()
        {
            var insurances = await _insuranceService.GetAllInsuranceAsync();
            return Ok(insurances);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Insurance>> GetInsurance(int id)
        {
            var insurance = await _insuranceService.GetInsuranceByIdAsync(id);
            if (insurance == null)
            {
                return NotFound();
            }
            return insurance;
        }

        [HttpGet("getInsuranceByVin/{vin}")]
        public async Task<ActionResult<Insurance>> GetInsuranceByVin(string vin)
        {
            var insurance = await _insuranceService.GetInsuranceByVinAsync(vin);
            if (insurance == null)
            {
                return NotFound($"No insurance found for the car with VIN {vin}.");
            }
            return Ok(insurance);
        }

        [HttpPut("updateInsuranceByVin/{vin}")]
        public async Task<ActionResult<Insurance>> UpdateInsuranceByVin(string vin, [FromBody] Insurance updatedInsurance)
        {
            var insurance = await _insuranceService.UpdateInsuranceByVinAsync(vin, updatedInsurance);
            if (insurance == null)
            {
                return NotFound($"No insurance found for the car with VIN {vin}.");
            }
            return Ok(insurance);
        }

        [HttpPost]
        public async Task<ActionResult<Insurance>> PostInsurance([FromBody] Insurance insurance)
        {
            var createdInsurance = await _insuranceService.AddInsuranceAsync(insurance);
            return CreatedAtAction(nameof(GetInsurance), new { id = createdInsurance.InsuranceId }, createdInsurance);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInsurance(int id)
        {
            var success = await _insuranceService.DeleteInsuranceAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}