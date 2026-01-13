using System.ComponentModel.DataAnnotations;

namespace PawsAndClaws.Models.ViewModels
{
    public class UpdateApplicationStatusRequest
    {
        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        [Range(1, 4)]
        public int CurrentStep { get; set; } = 1;
    }
}