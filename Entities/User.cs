using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DryvetrackTest.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }  // Store hashed password

        // Navigation property
        public List<Car> Cars { get; set; } = new List<Car>();
    }
}
