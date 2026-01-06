using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PawsAndClaws.Models.Entities;
using PawsAndClaws.Models.Identity;

namespace PawsAndClaws.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<OwnedPet> OwnedPets => Set<OwnedPet>();
        public DbSet<Pet> Pets => Set<Pet>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ These are UI-only models. Prevent EF from treating them as DB entities.
            builder.Ignore<PawsAndClaws.Models.AdoptionApplication>();
            builder.Ignore<PawsAndClaws.Models.ScheduledVisit>();
            builder.Ignore<PawsAndClaws.Models.ApplicationPageViewModel>();
        }
    }
}
