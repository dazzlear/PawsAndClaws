namespace PawsAndClaws.Models
{
    // UI model / placeholder para mag-compile yung Views.
    // Later papalitan mo ito ng EF Entity version.
    public class AdoptionApplication
    {
        public int Id { get; set; }

        public string PetName { get; set; } = "";
        public string PetBreed { get; set; } = "";
        public string PetImageUrl { get; set; } = "";

        public string Status { get; set; } = "Pending";

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // common fields na madalas ginagamit sa cards
        public string? Notes { get; set; }

        // aliases (para di ka ma-stuck kung ito pala gamit ng cshtml)
        public DateTime CreatedAt
        {
            get => SubmittedAt;
            set => SubmittedAt = value;
        }
    }
}
