namespace PawsAndClaws.Models.Entities
{
    public enum ApplicationStatus { Pending, Approved, Rejected }

    public class AdoptionApplication
    {
        public int Id { get; set; }

        public int PetId { get; set; }          // adoptable pet
        public string UserId { get; set; } = ""; // identity user

        public string Message { get; set; } = "";
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}