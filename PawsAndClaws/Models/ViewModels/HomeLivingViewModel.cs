namespace PawsAndClaws.Models.ViewModels
{
    public class HomeLivingViewModel
    {
        public required string Address { get; set; }
        public string LivingSituation { get; set; } = "House"; // Default selection
    }
}