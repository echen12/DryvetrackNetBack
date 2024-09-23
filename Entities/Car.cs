using Microsoft.EntityFrameworkCore;

namespace DryvetrackTest.Entities
{
    public class Car
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();  
        public string VIN { get; set; }   
        public string Color { get; set; } 

        public string VehicleType { get; set; } 
        public string Make { get; set; }   
        public string Model { get; set; }  
        public string ModelYear { get; set; } 
        public int CurrentMileage { get; set; } 
        public int UserId { get; set; }    
        public int InsuranceId { get; set; }

        
        
        
    }
}