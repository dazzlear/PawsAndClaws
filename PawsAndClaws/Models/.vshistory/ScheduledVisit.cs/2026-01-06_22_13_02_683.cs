namespace PawsAndClaws.Models
{
    public class ScheduledVisit
    {
        public int Id { get; set; }

        public string Title { get; set; } = "Shelter Visit";
        public string Location { get; set; } = "";
        public string Status { get; set; } = "Scheduled";

        public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;

        // aliases (para flexible sa cshtml)
        public DateTime Date
        {
            get => ScheduledAt;
            set => ScheduledAt = value;
        }

        public string TimeLabel => ScheduledAt.ToString("hh:mm tt");
    }
}