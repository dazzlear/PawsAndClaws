using Microsoft.AspNetCore.Mvc;
using PawsAndClaws.Models.Entities;
using PawsAndClaws.Models.ViewModels;

namespace PawsAndClaws.Controllers
{
    public class PetController : Controller
    {
        public IActionResult Inventory()
        {
            // seeded pets (Entities)
            var pets = new List<Pet>
            {
                new Pet { Name="Megatron", Breed="Aspin", Species="Dog", Age=2, Gender="Male", Size="Medium", Location="Quezon City, PH", Status="Pending", ImageUrl="/images/Megatron.jpg" },
                new Pet { Name="Asteroid Destroyer", Breed="Persian", Species="Cat", Age=1, Gender="Female", Size="Small", Location="Quezon City, PH", Status="Available", ImageUrl="/images/Asteroid.jpg" },
                new Pet { Name="Josie Megatron", Breed="Persian", Species="Cat", Age=1, Gender="Female", Size="Small", Location="Quezon City, PH", Status="Available", ImageUrl="/images/Josie.png" },
                new Pet { Name="Mini Melay", Breed="Chihuahua", Species="Dog", Age=1, Gender="Male", Size="Small", Location="Quezon City, PH", Status="Available", ImageUrl="/images/Melay.jpg" },
                new Pet { Name="Marites", Breed="Persian", Species="Cat", Age=3, Gender="Female", Size="Small", Location="Taguig City, PH", Status="Available", ImageUrl="/images/Marites.png" },
                new Pet { Name="Monami", Breed="Aspin", Species="Dog", Age=3, Gender="Female", Size="Medium", Location="Quezon City, PH", Status="Pending", ImageUrl="/images/Monami.jpg" },
                new Pet { Name="Greg Yapper", Breed="British Shorthair", Species="Cat", Age=1, Gender="Male", Size="Small", Location="Quezon City, PH", Status="Available", ImageUrl="/images/Greg.png" },
                new Pet { Name="Galaxy Annihilator", Breed="Persian", Species="Cat", Age=1, Gender="Female", Size="Small", Location="Quezon City, PH", Status="Pending", ImageUrl="/images/Galaxy.jpg" }
            };

            var cards = pets.Select((p, index) => new PetCardViewModel
            {
                Id = p.Id != 0 ? p.Id : (index + 1),   // give stable ids for seeded list
                Name = p.Name ?? "",
                Breed = p.Breed ?? "",
                Species = p.Species ?? "",
                Age = p.Age,
                Gender = p.Gender ?? "",
                Size = p.Size ?? "",
                Location = p.Location ?? "",
                Status = p.Status ?? "",
                ImageUrl = string.IsNullOrWhiteSpace(p.ImageUrl) ? "/images/pet-placeholder.jpg" : p.ImageUrl,
                Description = p.Description
            }).ToList();

            return View(cards);
        }
    }
}
