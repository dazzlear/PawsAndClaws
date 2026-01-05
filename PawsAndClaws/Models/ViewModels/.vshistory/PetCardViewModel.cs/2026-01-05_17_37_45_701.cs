namespace PawsAndClaws.Models
{
    public class PerCardViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Description { get; set; } 
    }
}