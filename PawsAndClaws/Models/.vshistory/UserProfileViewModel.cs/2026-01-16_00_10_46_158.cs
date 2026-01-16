using PawsAndClaws.Models.Entities;
using PawsAndClaws.Models.ViewModels;

namespace PawsAndClaws.Models
{
    public class UserProfileViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? LivingSituation { get; set; }
        public string? FullAddress { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int CurrentPetCount { get; set; }

        public List<OwnedPet> OwnedPets { get; set; } = new();
        public AddPetViewModel NewPet { get; set; } = new();
    }
}
