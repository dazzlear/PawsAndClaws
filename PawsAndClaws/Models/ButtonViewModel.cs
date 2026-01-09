namespace PawsAndClaws.Models
{
    public class ButtonViewModel
    {
        public required string Text { get; set; }
        public string? Href { get; set; }     
        public string? Icon { get; set; }      // Nullable because not all buttons have icons
        public string Variant { get; set; } = "primary"; // Default value
        public string ClassName { get; set; } = string.Empty;
        public string Type { get; set; } = "button";
        public string? Id { get; set; }
        public string? DataModalTarget { get; set; }
    }
}