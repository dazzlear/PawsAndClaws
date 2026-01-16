using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawsAndClaws.Data;
using PawsAndClaws.Models.Entities;
using PawsAndClaws.Models.ViewModels;
using System.Diagnostics;
using System.Security.Claims;


namespace PawsAndClaws.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        // 1) MAIN LANDING PAGE (cards)
        public async Task<IActionResult> Index()
        {
            //await SeedPetsIfEmptyAsync();

            var pets = await _db.Pets
                .AsNoTracking()
                .Select(p => new PetCardViewModel
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
                    ImageUrl = p.ImageUrl ?? "",
                    Description = p.Description
                })
                .ToListAsync();

            return View(pets);
        }

        // 2) PET DETAILS (entity)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            var pet = await _db.Pets
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null) return NotFound();

            pet.Description ??= "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt...";

            bool alreadyApplied = false;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    alreadyApplied = await _db.AdoptionApplications
                        .AsNoTracking()
                        .AnyAsync(a => a.UserId == userId && a.PetId == id);
                }
            }

            ViewBag.AlreadyApplied = alreadyApplied;

            return View(pet);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

    }
}