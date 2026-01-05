namespace PawsAndClaws.Models.Entities
{
    public class OwnedPet
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";
        public string Breed { get; set; } = "";
        public string? Age { get; set; }
        public string? Sex { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}