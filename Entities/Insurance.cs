namespace DryvetrackTest.Entities
{
    public class Insurance
    {
        public int InsuranceId { get; set; } // Primary Key
        public string LicensePlate { get; set; } // License plate number
        public string Provider { get; set; } // Insurance provider name
        public DateTime ExpiryDate { get; set; } // Expiry date of the insurance

        

    }
}
