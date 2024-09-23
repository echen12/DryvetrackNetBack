using DryvetrackTest.Data;
using DryvetrackTest.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DryvetrackTest.Services
{
    public class InsuranceService
    {
        private readonly DataContext _context;

        public InsuranceService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Insurance>> GetAllInsuranceAsync()
        {
            return await _context.Insurance.ToListAsync();
        }

        public async Task<Insurance> GetInsuranceByIdAsync(int id)
        {
            return await _context.Insurance.FindAsync(id);
        }

        public async Task<Insurance> GetInsuranceByVinAsync(string vin)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.VIN == vin);
            if (car == null) return null;
            return await _context.Insurance.FirstOrDefaultAsync(i => i.InsuranceId == car.InsuranceId);
        }

        public async Task<Insurance> UpdateInsuranceByVinAsync(string vin, Insurance updatedInsurance)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.VIN == vin);
            if (car == null) return null;

            var insurance = await _context.Insurance.FirstOrDefaultAsync(i => i.InsuranceId == car.InsuranceId);
            if (insurance == null) return null;

            insurance.LicensePlate = updatedInsurance.LicensePlate;
            insurance.Provider = updatedInsurance.Provider;
            insurance.ExpiryDate = updatedInsurance.ExpiryDate;

            await _context.SaveChangesAsync();
            return insurance;
        }

        public async Task<Insurance> AddInsuranceAsync(Insurance insurance)
        {
            _context.Insurance.Add(insurance);
            await _context.SaveChangesAsync();
            return insurance;
        }

        public async Task<bool> DeleteInsuranceAsync(int id)
        {
            var insurance = await _context.Insurance.FindAsync(id);
            if (insurance == null) return false;

            _context.Insurance.Remove(insurance);
            await _context.SaveChangesAsync();
            return true;
        }

        // You can also add more methods as needed
    }
}