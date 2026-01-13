using Microsoft.AspNetCore.Mvc;
using PawsAndClaws.Data;
using PawsAndClaws.Models.ViewModels;

namespace PawsAndClaws.Controllers
{
    public class PetController : Controller
    {
        public IActionResult Inventory()
        {
            var pets = PetSeedData.Pets;

            var cards = pets.Select(p => new PetCardViewModel
            {
                Id = p.Id,
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
