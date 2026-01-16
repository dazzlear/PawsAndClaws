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
                Description = p.Description,
                IsAdminMode = true
            }).ToList();

            return View(cards);
        }

        [HttpGet]
        public IActionResult GetPetData(int id)
        {
            var pet = PetSeedData.Pets.FirstOrDefault(p => p.Id == id);
            if (pet == null)
                return NotFound();

            return Json(new
            {
                id = pet.Id,
                name = pet.Name,
                breed = pet.Breed,
                age = pet.Age,
                gender = pet.Gender,
                species = pet.Species,
                location = pet.Location,
                description = pet.Description,
                imageUrl = pet.ImageUrl
            });
        }

        [HttpPost]
        public IActionResult EditPet(int id)
        {
            var pet = PetSeedData.Pets.FirstOrDefault(p => p.Id == id);
            if (pet == null)
                return NotFound();

            // Update pet properties from form data
            pet.Name = Request.Form["Name"];
            pet.Breed = Request.Form["Breed"];
            if (int.TryParse(Request.Form["Age"], out int age))
                pet.Age = age;
            pet.Gender = Request.Form["Gender"];
            pet.Species = Request.Form["Species"];
            pet.Location = Request.Form["Location"];
            pet.Description = Request.Form["Description"];

            // Handle photo upload if provided
            if (Request.Form.Files.Count > 0)
            {
                var photoFile = Request.Form.Files[0];
                if (photoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{pet.Id}_{DateTime.UtcNow.Ticks}.jpg";
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        photoFile.CopyTo(stream);
                    }
                    pet.ImageUrl = $"/uploads/{fileName}";
                }
            }

            return RedirectToAction(nameof(Inventory));
        }

        [HttpDelete]
        public IActionResult DeletePet(int id)
        {
            var pet = PetSeedData.Pets.FirstOrDefault(p => p.Id == id);
            if (pet == null)
                return NotFound();

            PetSeedData.Pets.Remove(pet);
            return Ok();
        }
    }
}
