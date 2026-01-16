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

        // ✅ Only ONE constructor, using YOUR actual context: AppDbContext
        public PetController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Inventory()
        {
            var pets = await _db.Pets.AsNoTracking().ToListAsync();

            var cards = pets.Select(p => new PetCardViewModel
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
                ImageUrl = string.IsNullOrWhiteSpace(p.ImageUrl) ? "/images/pet-placeholder.jpg" : p.ImageUrl,
                Description = p.Description,
                IsAdminMode = true
            }).ToList();

            return View(cards);
        }

        // ✅ GET: /Pet/GetPetData/5
        // ✅ GET: /Pet/GetPetData/5
        [HttpGet]
        public async Task<IActionResult> GetPetData(int id)
        {
            var pet = await _db.Pets
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    breed = p.Breed,
                    age = p.Age,
                    gender = p.Gender,
                    species = p.Species,
                    location = p.Location,
                    description = p.Description,
                    imageUrl = p.ImageUrl,     // ✅ add this
                    size = p.Size,             // optional (if you add size in edit modal)
                    status = p.Status          // optional
                })
                .FirstOrDefaultAsync();

            if (pet == null) return NotFound();
            return Json(pet);
        }


        // ✅ POST: /Pet/DeletePet/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePet(int id)
        {
            var pet = await _db.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            _db.Pets.Remove(pet);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPet(Pet model, IFormFile? PetPhoto, string? ExistingImageUrl)
        {
            var pet = await _db.Pets.FindAsync(model.Id);
            if (pet == null) return NotFound();

            // Update fields
            pet.Name = model.Name?.Trim();
            pet.Breed = model.Breed?.Trim();
            pet.Age = model.Age;
            pet.Gender = model.Gender?.Trim();
            pet.Species = model.Species?.Trim();
            pet.Location = model.Location?.Trim();
            pet.Description = model.Description;

            // Image handling (optional)
            if (PetPhoto != null && PetPhoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(PetPhoto.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await PetPhoto.CopyToAsync(stream);

                pet.ImageUrl = $"/uploads/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(ExistingImageUrl))
            {
                // keep previous
                pet.ImageUrl = ExistingImageUrl;
            }

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pet updated successfully!";
            return RedirectToAction(nameof(Inventory));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPet(AddPetViewModel vm, IFormFile? PetPhoto)
        {
            if (!ModelState.IsValid)
            {
                TempData["AddPetError"] = "Please fill in required fields (Name and Species).";
                return RedirectToAction(nameof(Inventory), new { add = 1 });
            }

            string imageUrl = "/images/pet-placeholder.jpg";

            if (PetPhoto != null && PetPhoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(PetPhoto.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await PetPhoto.CopyToAsync(stream);

                imageUrl = $"/uploads/{fileName}";
            }

            var pet = new Pet
            {
                Name = vm.Name.Trim(),
                Species = vm.Species.Trim(),
                Breed = (vm.Breed ?? "").Trim(),
                Age = vm.Age,
                Gender = (vm.Gender ?? "").Trim(),
                Size = (vm.Size ?? "").Trim(),
                Location = (vm.Location ?? "").Trim(),
                Description = vm.Description,
                ImageUrl = imageUrl,
                Status = "Available"
            };

            _db.Pets.Add(pet);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pet added successfully!";
            return RedirectToAction(nameof(Inventory));
        }
    }
}