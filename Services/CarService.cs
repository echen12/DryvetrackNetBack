using DryvetrackTest.Data;
using DryvetrackTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace DryvetrackTest.Services
{
    public class CarService
    {
        private readonly DataContext _context;

        public CarService(DataContext context)
        {
            _context = context;
        }

        public async Task<Car> AddCar(User user, dynamic generalObject)
        {
            var insurance = new Insurance
            {
                LicensePlate = generalObject.GetProperty("plateNumber").GetString(),
                Provider = generalObject.GetProperty("insuranceProvider").GetString(),
                ExpiryDate = generalObject.GetProperty("insuranceExpiryDate").GetDateTime()
            };

            _context.Insurance.Add(insurance);
            await _context.SaveChangesAsync();

            var car = new Car
            {
                VIN = generalObject.GetProperty("vin").GetString(),
                Make = generalObject.GetProperty("make").GetString(),
                Model = generalObject.GetProperty("model").GetString(),
                Color = generalObject.GetProperty("color").GetString(),
                VehicleType = generalObject.GetProperty("vehicleType").GetString(),
                ModelYear = generalObject.GetProperty("modelYear").GetString(),
                CurrentMileage = int.Parse(generalObject.GetProperty("mileage").GetString()),
                UserId = user.UserId,
                InsuranceId = insurance.InsuranceId
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return car;
        }

        public async Task<IEnumerable<Car>> GetCarsByUserId(int userId)
        {
            return await _context.Cars.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<Car> GetCarByVin(string vin)
        {
            return await _context.Cars.SingleOrDefaultAsync(c => c.VIN == vin);
        }

        public async Task UpdateMileage(string vin, int newMileage)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.VIN == vin);
            if (car != null)
            {
                car.CurrentMileage = newMileage;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCar(string id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                if (car.InsuranceId != null)
                {
                    var insurance = await _context.Insurance.FindAsync(car.InsuranceId);
                    if (insurance != null)
                    {
                        _context.Insurance.Remove(insurance);
                    }
                }

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }
    }
}