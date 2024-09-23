using Microsoft.EntityFrameworkCore;

namespace DryvetrackTest.Entities
{
    public class Car
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();  // Unique identifier for the car
        public string VIN { get; set; }   // Vehicle Identification Number
        public string Color { get; set; } // Color of the car

        public string VehicleType { get; set; } // Type of the vehicle (e.g., sedan, SUV)
        public string Make { get; set; }   // Car make (e.g., Toyota, Ford)
        public string Model { get; set; }  // Car model (e.g., Camry, Focus)
        public string ModelYear { get; set; } // Model year of the car
        public int CurrentMileage { get; set; } // Current mileage of the car
        public int UserId { get; set; }    // Foreign key to associate with User
        public int InsuranceId { get; set; }

        // Foreign Key to Insurance
        
        
    }
}