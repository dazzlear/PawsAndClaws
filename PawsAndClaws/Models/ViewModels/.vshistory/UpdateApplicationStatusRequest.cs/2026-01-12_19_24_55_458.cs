using System.ComponentModel.DataAnnotations;

namespace PawsAndClaws.Models.ViewModels
{
    public class UpdateApplicationStatusRequest : IValidatableObject
    {
        [Required]
        public int ApplicationId { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "PENDING";

        [Range(1, 4)]
        public int CurrentStep { get; set; } = 1;

        // Helper (use this in your controller before saving)
        public string NormalizedStatus =>
            (Status ?? "").Trim().ToUpperInvariant();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var allowed = new HashSet<string>
            {
                "REVIEW", "PENDING", "APPROVED", "REJECTED", "SCHEDULED", "COMPLETED", "CANCELLED"
            };

            var s = NormalizedStatus;

            if (!allowed.Contains(s))
            {
                yield return new ValidationResult(
                    "Invalid status value.",
                    new[] { nameof(Status) }
                );
            }

            // Optional rule: rejected/cancelled shouldn't move stages
            if ((s == "REJECTED" || s == "CANCELLED") && CurrentStep != 1)
            {
                yield return new ValidationResult(
                    "Rejected/Cancelled applications must stay at Stage 1.",
                    new[] { nameof(CurrentStep) }
                );
            }
        }
    }
}