namespace PawsAndClaws.Models.Entities
{
    public enum ApplicationStatus { Pending, Approved, Rejected }

    public class AdoptionApplication
    {
        public int Id { get; set; }

        public int PetId { get; set; }
        public Pet? Pet { get; set; } 

        public string UserId { get; set; } = "";
        public string Message { get; set; } = "";

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
