using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawsAndClaws.Data;
using PawsAndClaws.Models.Entities;
using PawsAndClaws.Models.Identity;
using PawsAndClaws.Models.ViewModels;

namespace PawsAndClaws.Controllers
{
    [Authorize]
    public class AdoptionController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdoptionController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int petId, string message)
        {
            message = (message ?? "").Trim();

            // validate message length
            if (message.Length < 20)
            {
                TempData["ErrorMessage"] = "Please provide a bit more detail (at least 20 characters).";

               
                var petName = await _db.Pets
                    .AsNoTracking()
                    .Where(p => p.Id == petId)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrWhiteSpace(petName))
                    return RedirectToAction("Details", "Home", new { id = petId });

                return RedirectToAction("Index", "Home");
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            // prevents FK error
            var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == petId);
            if (pet == null)
            {
                TempData["ErrorMessage"] = "Pet not found.";
                return RedirectToAction("Index", "Home");
            }

            // prevent duplicate application per user+pet
            var alreadyApplied = await _db.AdoptionApplications
                .AnyAsync(a => a.UserId == userId && a.PetId == petId);

            if (alreadyApplied)
            {
                TempData["ErrorMessage"] = "You already submitted an application for this pet.";
                return RedirectToAction(nameof(MyApplications));
            }

            var app = new AdoptionApplication
            {
                UserId = userId,
                PetId = petId,
                Message = message,
                Status = "Pending",
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.AdoptionApplications.Add(app);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Application submitted!";
            return RedirectToAction(nameof(MyApplications));
        }

        [HttpGet]
        public async Task<IActionResult> MyApplications(string status = "All")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var userId = user.Id;

            var appsQuery = _db.AdoptionApplications
                .AsNoTracking()
                .Include(a => a.Pet)
                .Where(a => a.UserId == userId);

            var allApps = await appsQuery.ToListAsync();

            var filtered = allApps.AsEnumerable();
            if (!string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                filtered = filtered.Where(a =>
                    string.Equals(a.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            var vm = new ApplicationPageViewModel
            {
                UserName = GetDisplayName(user),

                Applications = filtered
                    .OrderByDescending(a => a.CreatedAtUtc)
                    .Select(a => new ApplicationCardViewModel
                    {
                        PetName = a.Pet?.Name ?? "Unknown",
                        Breed = a.Pet?.Breed ?? "",
                        Details = $"{a.Pet?.Species} • {a.Pet?.Age} years • {a.Pet?.Gender}",
                        ImageUrl = a.Pet?.ImageUrl ?? "",
                        Status = a.Status,
                        CurrentStep = StatusToStep(a.Status)
                    })
                    .ToList(),

                Visits = new List<ScheduledVisitViewModel>(), // add later
                StatusCounts = BuildCounts(allApps)
            };

            return View(vm);
        }

        private static string GetDisplayName(ApplicationUser user)
        {
         
            if (!string.IsNullOrWhiteSpace(user.FirstName))
                return user.FirstName;

            // fallback: take left part before "@"
            var candidate = user.UserName ?? user.Email ?? "";
            if (!string.IsNullOrWhiteSpace(candidate) && candidate.Contains('@'))
                return candidate.Split('@')[0];

            return string.IsNullOrWhiteSpace(candidate) ? "User" : candidate;
        }

        private static int StatusToStep(string status)
        {
            return (status ?? "").Trim().ToLower() switch
            {
                "pending" => 1,
                "approved" => 2,
                "scheduled" => 3,
                "completed" => 4,
                _ => 1
            };
        }

        private static Dictionary<string, int> BuildCounts(IEnumerable<AdoptionApplication> apps)
        {
            var list = apps.ToList();

            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["All"] = list.Count,
                ["Pending"] = 0,
                ["Approved"] = 0,
                ["Scheduled"] = 0,
                ["Completed"] = 0,
                ["Rejected"] = 0,
                ["Cancelled"] = 0
            };

            foreach (var g in list.GroupBy(a => (a.Status ?? "Pending").Trim()))
                dict[g.Key] = g.Count();

            return dict;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageApplications(string status = "All")
        {
            var appsQuery = _db.AdoptionApplications
                .AsNoTracking()
                .Include(a => a.Pet)
                .Include(a => a.User);

            var allApps = await appsQuery.ToListAsync();

            var filtered = allApps.AsEnumerable();
            if (!string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                filtered = filtered.Where(a =>
                    string.Equals(a.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            var vm = new ApplicationPageViewModel
            {
                UserName = "Admin",
                Applications = filtered
                    .OrderByDescending(a => a.CreatedAtUtc)
                    .Select(a => new ApplicationCardViewModel
                    {
                        ApplicationId = a.Id,          // for updates
                        PetId = a.PetId,
                        PetName = a.Pet?.Name ?? "Unknown",
                        Breed = a.Pet?.Breed ?? "",
                        Details = $"{a.Pet?.Species} • {a.Pet?.Age} years • {a.Pet?.Gender}",
                        ImageUrl = a.Pet?.ImageUrl ?? "",
                        Status = a.Status,
                        CurrentStep = StatusToStep(a.Status)
                    })
                    .ToList(),
                Visits = new List<ScheduledVisitViewModel>(),
                StatusCounts = BuildCounts(allApps)
            };

            return View(vm);
        }
    }
}