using Microsoft.EntityFrameworkCore;
using PawsAndClaws.Models.Entities;

namespace PawsAndClaws.Data
{
    public static class PetDbSeeder
    {
        public static async Task SeedPetsAsync(AppDbContext db)
        {
            // seed only if empty
            if (await db.Pets.AnyAsync()) return;

            var seedPets = PetSeedData.Pets.Select(p => new Pet
            {
                Name = p.Name,
                Breed = p.Breed,
                Species = p.Species,
                Age = p.Age,
                Gender = p.Gender,
                Size = p.Size,
                Location = p.Location,
                Status = string.IsNullOrWhiteSpace(p.Status) ? "Available" : p.Status,
                ImageUrl = string.IsNullOrWhiteSpace(p.ImageUrl) ? "/images/pet-placeholder.jpg" : p.ImageUrl,
                Description = p.Description
            }).ToList();

            db.Pets.AddRange(seedPets);
            await db.SaveChangesAsync();
        }
    }
}
