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

        public DbSet<AdoptionApplication> AdoptionApplications => Set<AdoptionApplication>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // one application per user per pet
            builder.Entity<AdoptionApplication>()
                .HasIndex(a => new { a.UserId, a.PetId })
                .IsUnique();

            builder.Entity<AdoptionApplication>()
                .HasOne(a => a.Pet)
                .WithMany(p => p.Applications)
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            // owned pets
            builder.Entity<OwnedPet>()
                .HasOne(op => op.User)
                .WithMany(u => u.OwnedPets)
                .HasForeignKey(op => op.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OwnedPet>()
                .HasIndex(op => op.UserId);

            builder.Entity<OwnedPet>()
                .Property(p => p.CreatedAtUtc)
                .HasColumnName("CreatedAt");
        }
    }
}
