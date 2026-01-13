namespace PawsAndClaws.Models.ViewModels
{
    public class ApplicationPageViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<ApplicationCardViewModel> Applications { get; set; } = new();
        public List<ScheduledVisitViewModel> Visits { get; set; } = new();
        public Dictionary<string, int> StatusCounts { get; set; } = new();
        public string UserName { get; set; } = "";
    }

    public class ApplicationCardViewModel
    {
        public string PetName { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CurrentStep { get; set; }
    }

    public class ScheduledVisitViewModel
    {
        public string PetName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Schedule { get; set; }
    }
}