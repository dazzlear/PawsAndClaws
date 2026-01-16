using System.ComponentModel.DataAnnotations;

namespace PawsAndClaws.Models.ViewModels
{
    public class AddPetViewModel
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Species { get; set; } = "";

        public string? Breed { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }

        [Required]
        public string Size { get; set; } = "";
        public string? Location { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}
