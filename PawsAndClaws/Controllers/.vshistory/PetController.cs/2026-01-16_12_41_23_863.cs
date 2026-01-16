using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawsAndClaws.Data;
using PawsAndClaws.Models.Entities;
using PawsAndClaws.Models.ViewModels;

namespace PawsAndClaws.Controllers
{
    public class PetController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public PetController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ? Inventory now reads from DB (not PetSeedData)
        [HttpGet]
        public async Task<IActionResult> Inventory()
        {
            var pets = await _db.Pets.AsNoTracking().ToListAsync();

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

        // ? AddPet saves to DB (matches your modal form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPet(AddPetViewModel vm, IFormFile? PetPhoto)
        {
            if (!ModelState.IsValid)
            {
                // simplest: return back to inventory (or you can return the view with errors)
                return RedirectToAction(nameof(Inventory));
            }

            // ----- Upload photo (optional) -----
            string imageUrl = "/images/pet-placeholder.jpg";

            if (PetPhoto != null && PetPhoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(PetPhoto.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await PetPhoto.CopyToAsync(stream);

                imageUrl = $"/uploads/{fileName}";
            }

            // ----- Save to DB -----
            var pet = new Pet
            {
                Name = vm.Name.Trim(),
                Species = vm.Species.Trim(),
                Breed = (vm.Breed ?? "").Trim(),
                Age = vm.Age,
                Gender = (vm.Gender ?? "").Trim(),
                Location = (vm.Location ?? "").Trim(),
                Description = vm.Description,
                ImageUrl = imageUrl,

                // not in AddPetViewModel, so default here:
                Size = "",                 // or "Small/Medium/Large" if you want a default
                Status = "Available"       // good default for inventory
            };

            _db.Pets.Add(pet);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Inventory));
        }

        // ? Fetch pet data from DB for edit modal
        [HttpGet]
        public async Task<IActionResult> GetPetData(int id)
        {
            var pet = await _db.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null) return NotFound();

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
                imageUrl = pet.ImageUrl,
                status = pet.Status,
                size = pet.Size
            });
        }
    }
}