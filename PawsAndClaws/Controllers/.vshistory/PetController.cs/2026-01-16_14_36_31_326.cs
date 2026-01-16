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

        private readonly ApplicationDbContext _context;
        public PetController(ApplicationDbContext context) => _context = context;

        // GET: /Pet/GetPetData/5
        [HttpGet]
        public async Task<IActionResult> GetPetData(int id)
        {
            var pet = await _context.Pets
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new {
                    id = p.Id,
                    name = p.Name,
                    breed = p.Breed,
                    age = p.Age,
                    gender = p.Gender,
                    species = p.Species,
                    location = p.Location,
                    description = p.Description
                })
                .FirstOrDefaultAsync();

            if (pet == null) return NotFound();
            return Json(pet);
        }

        // POST: /Pet/DeletePet/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePet(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPet(AddPetViewModel vm, IFormFile? PetPhoto)
        {
            // IMPORTANT: if invalid, don’t silently redirect with no feedback
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