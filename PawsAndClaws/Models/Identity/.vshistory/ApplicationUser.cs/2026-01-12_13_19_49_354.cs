using Microsoft.AspNetCore.Identity;

namespace PawsAndClaws.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public bool HasOtherPets { get; set; }

        public string Address { get; set; } = "";
        public string LivingSituation { get; set; } = "House";

        // saves Step3 list in DB (JSON)
        public string OtherPetsJson { get; set; } = "[]";
    }
}
