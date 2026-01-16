// Models/Entities/OwnedPet.cs
using PawsAndClaws.Models.Identity;

namespace PawsAndClaws.Models.Entities
{
    public class OwnedPet
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public ApplicationUser? User { get; set; }

        public string Name { get; set; } = default!;
        public string Species { get; set; } = default!;
        public string Gender { get; set; } = default!;
        public string? Breed { get; set; }
        public int Age { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
