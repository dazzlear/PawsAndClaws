using System.ComponentModel.DataAnnotations;
using PawsAndClaws.Models.Identity;

namespace PawsAndClaws.Models.Entities
{
    public class AdoptionApplication
    {
        public int Id { get; set; }  // primary Key

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        public int PetId { get; set; }
        public Pet? Pet { get; set; }

        [Required, MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
