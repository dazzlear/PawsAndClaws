using System.Collections.Generic;

namespace PawsAndClaws.Models
{
    public class ApplicationPageViewModel
    {
        public string SelectedStatus { get; set; } = "All";

        public List<string> StatusOptions { get; set; } = new()
        {
            "All", "Pending", "Approved", "Rejected", "Completed"
        };

        public List<AdoptionApplication> Applications { get; set; } = new();
        public List<ScheduledVisit> ScheduledVisits { get; set; } = new();

        // aliases (kung ito pala ginagamit ng view)
        public List<ScheduledVisit> Visits
        {
            get => ScheduledVisits;
            set => ScheduledVisits = value;
        }
    }
}