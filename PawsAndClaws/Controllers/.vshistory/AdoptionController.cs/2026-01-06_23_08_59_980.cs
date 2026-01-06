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

            if (message.Length < 20)
            {
                TempData["ErrorMessage"] = "Please provide a bit more detail (at least 20 characters).";
                return RedirectToAction("Details", "Home", new { id = petId });
            }

            var userId = _userManager.GetUserId(User)!;

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
                filtered = filtered.Where(a => string.Equals(a.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            var vm = new ApplicationPageViewModel
            {
                UserName = user.UserName ?? user.Email ?? "User",
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
                Visits = new List<ScheduledVisitViewModel>(), // add later when you build visits table
                StatusCounts = BuildCounts(allApps)
            };

            return View(vm);
        }

        private static int StatusToStep(string status)
        {
            return status.ToLower() switch
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

            foreach (var g in list.GroupBy(a => a.Status ?? "Pending"))
                dict[g.Key] = g.Count();

            return dict;
        }
    }
}