namespace PawsAndClaws.Models.ViewModels
{
    public class ApplicationPageViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<ApplicationCardViewModel> Applications { get; set; } = new();
        public List<ScheduledVisitViewModel> Visits { get; set; } = new();
        public Dictionary<string, int> StatusCounts { get; set; } = new();
    }

    public class ApplicationCardViewModel
    {
        public int ApplicationId { get; set; }  
        public int PetId { get; set; }      

        public string PetName { get; set; } = "";
        public string Breed { get; set; } = "";
        public string Details { get; set; } = "";
        public string ImageUrl { get; set; } = "";

        public string Status { get; set; } = "Pending";
        public int CurrentStep { get; set; } = 1;
    }

    public class ScheduledVisitViewModel
    {
        public string PetName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Schedule { get; set; }
    }
}