namespace PawsAndClaws.Models
{
    public class ApplicationPageViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<AdoptionApplication> Applications { get; set; } = new();
        public List<ScheduledVisit> Visits { get; set; } = new();
        public Dictionary<string, int> StatusCounts { get; set; } = new();
    }

    public class AdoptionApplication
    {
        public string PetName { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CurrentStep { get; set; } // 1-4
    }

    public class ScheduledVisit
    {
        public string PetName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Schedule { get; set; }
    }

}