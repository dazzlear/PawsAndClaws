using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawsAndClaws.Data;
using PawsAndClaws.Models.Entities;
using PawsAndClaws.Models.ViewModels;

namespace PawsAndClaws.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        // 1. MAIN LANDING PAGE
        public async Task<IActionResult> Index()
        {
            // ✅ seed once (only if empty)
            await SeedPetsIfEmptyAsync();

            // ✅ load from DB
            var pets = await _db.Pets
                .AsNoTracking()
                .Select(p => new PetCardViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Breed = p.Breed,
                    Species = p.Species,
                    Age = p.Age,
                    Gender = p.Gender,
                    Size = p.Size,
                    Location = p.Location,
                    Status = p.Status,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description
                })
                .ToListAsync();

            return View(pets);
        }

        // 2. PET DETAILS / PROFILE VIEW
        public async Task<IActionResult> Details(int id)
        {
            await SeedPetsIfEmptyAsync();

            var pet = await _db.Pets
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PetCardViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Breed = p.Breed,
                    Species = p.Species,
                    Age = p.Age,
                    Gender = p.Gender,
                    Size = p.Size,
                    Location = p.Location,
                    Status = p.Status,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description
                })
                .FirstOrDefaultAsync();

            if (pet == null) return NotFound();

            // Optional: if description is empty, force a placeholder
            pet.Description ??= "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt...";

            return View(pet);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // SEEDER 
        private async Task SeedPetsIfEmptyAsync()
        {
            if (await _db.Pets.AnyAsync()) return;

            var samples = GetSamplePets();

            var entities = samples.Select(x => new Pet
            {
                Name = x.Name,
                Breed = x.Breed,
                Species = x.Species,
                Age = x.Age,
                Gender = x.Gender,
                Size = x.Size,
                Location = x.Location,
                Status = x.Status,
                ImageUrl = x.ImageUrl,
                Description = x.Description ?? "Lorem ipsum dolor sit amet, consectetur adipiscing elit..."
            }).ToList();

            _db.Pets.AddRange(entities);
            await _db.SaveChangesAsync();
        }

        // 3. SAMPLE DATA SOURCE 
        private List<PetCardViewModel> GetSamplePets()
        {
            return new List<PetCardViewModel>
            {
                new PetCardViewModel
                {
                    Name = "Megatron",
                    Breed = "Aspin",
                    Species = "Dog",
                    Age = 2,
                    Gender = "Male",
                    Size = "Medium",
                    Location = "Quezon City, PH",
                    Status = "Pending",
                    ImageUrl = "/images/Megatron.jpg",
                    Description = "Friendly dog looking for a home."
                },
                new PetCardViewModel
                {
                    Name = "Asteroid Destroyer",
                    Breed = "Persian",
                    Species = "Cat",
                    Age = 1,
                    Gender = "Female",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Asteroid.jpg",
                    Description = "Sweet and playful cat."
                },
                new PetCardViewModel
                {
                    Name = "Josie Megatron",
                    Breed = "Persian",
                    Species = "Cat",
                    Age = 1,
                    Gender = "Female",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Josie.png",
                    Description = "Calm cat, loves naps."
                },
                new PetCardViewModel
                {
                    Name = "Mini Melay",
                    Breed = "Chihuahua",
                    Species = "Dog",
                    Age = 1,
                    Gender = "Male",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Melay.jpg",
                    Description = "Small but energetic!"
                },
                new PetCardViewModel
                {
                    Name = "Marites",
                    Breed = "Persian",
                    Species = "Cat",
                    Age = 3,
                    Gender = "Female",
                    Size = "Small",
                    Location = "Taguig City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Marites.png",
                    Description = "Very affectionate and gentle."
                },
                new PetCardViewModel
                {
                    Name = "Monami",
                    Breed = "Aspin",
                    Species = "Dog",
                    Age = 3,
                    Gender = "Female",
                    Size = "Medium",
                    Location = "Quezon City, PH",
                    Status = "Pending",
                    ImageUrl = "/images/Monami.jpg",
                    Description = "Loyal dog who loves walks."
                },
                new PetCardViewModel
                {
                    Name = "Greg Yapper",
                    Breed = "British Shorthair",
                    Species = "Cat",
                    Age = 1,
                    Gender = "Male",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Greg.png",
                    Description = "Curious cat, likes to explore."
                },
                new PetCardViewModel
                {
                    Name = "Galaxy Annihilator",
                    Breed = "Persian",
                    Species = "Cat",
                    Age = 1,
                    Gender = "Female",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Pending",
                    ImageUrl = "/images/Galaxy.jpg",
                    Description = "Shy but warms up fast."
                }
            };
        }
    }
}