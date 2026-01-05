namespace PawsAndClaws.Models;

public class BadgeViewModel
{
    public required string Text { get; set; }
    public string Variant { get; set; } = "neutral"; // success, warning, danger, info, neutral, primary
    public string? ClassName { get; set; }
}
