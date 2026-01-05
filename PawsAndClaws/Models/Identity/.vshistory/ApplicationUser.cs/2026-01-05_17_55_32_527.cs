using Microsoft.AspNetCore.Identity;

namespace PawsAndClaws.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // from Register step 1
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        // from Register step 4
        public string Address { get; set; } = "";
        public string LivingSituation { get; set; } = "House";

        // step 2
        public bool HasOtherPets { get; set; }
    }
}