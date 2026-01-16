using PawsAndClaws.Models.Identity;

namespace PawsAndClaws.Models.Entities
{
    public class OwnedPet
    {
        public int Id { get; set; }

        // FK
        public string UserId { get; set; } = "";
        public ApplicationUser? User { get; set; }

        // Step 3 fields
        public string Name { get; set; } = "";
        public string Species { get; set; } = "";   
        public string Gender { get; set; } = "";    
        public string Breed { get; set; } = "";
        public int Age { get; set; }               

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
