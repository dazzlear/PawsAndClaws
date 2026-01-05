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
        public DbSet<AdoptionApplication> AdoptionApplications => Set<AdoptionApplication>();
        public DbSet<Pet> Pets => Set<Pet>();
    }
    protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AdoptionApplication>()
                .HasIndex(a => new { a.UserId, a.PetId })
                .IsUnique();

            builder.Entity<AdoptionApplication>()
                .HasOne(a => a.Pet)
                .WithMany(p => p.Applications)
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }