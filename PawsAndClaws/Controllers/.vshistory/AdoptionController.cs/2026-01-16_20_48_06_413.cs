using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawsAndClaws.Data;
using PawsAndClaws.Models.Entities;
using PawsAndClaws.Models.Identity;
using PawsAndClaws.Models.ViewModels;
using System.Security.Claims;

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

        private static string Norm(string? s) => (s ?? "").Trim().ToUpperInvariant();

        private static string ComputeInventoryStatusFromApps(IEnumerable<AdoptionApplication> apps)
        {
            var statuses = apps.Select(a => Norm(a.Status)).ToList();

            // Priority:
            // ADOPTED -> Adopted
            if (statuses.Contains("ADOPTED"))
                return "Adopted";

            // PENDING or APPROVED -> Pending
            if (statuses.Any(s => s == "PENDING" || s == "APPROVED"))
                return "Pending";

            // REVIEW / REJECTED / CANCELLED only -> Available
            return "Available";
        }

        private async Task RecomputePetInventoryStatusAsync(int petId)
        {
            var pet = await _db.Pets
                .Include(p => p.Applications)
                .FirstOrDefaultAsync(p => p.Id == petId);

            if (pet == null) return;

            pet.Status = ComputeInventoryStatusFromApps(pet.Applications);
            await _db.SaveChangesAsync();
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

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            // Load pet (TRACKED) because we’ll validate inventory status
            var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == petId);
            if (pet == null)
            {
                TempData["ErrorMessage"] = "Pet not found.";
                return RedirectToAction("Index", "Home");
            }

            // Validation: block if inventory is Pending/Adopted
            var inv = Norm(pet.Status);
            if (inv == "PENDING" || inv == "ADOPTED")
            {
                TempData["ErrorMessage"] = "This pet is currently unavailable for new applications.";
                return RedirectToAction("Details", "Home", new { id = petId });
            }

            // IMPORTANT: one row per (user, pet). Reuse cancelled record.
            var existing = await _db.AdoptionApplications
                .FirstOrDefaultAsync(a => a.UserId == userId && a.PetId == petId);

            if (existing != null)
            {
                if (Norm(existing.Status) == "CANCELLED")
                {
                    // Re-activate the cancelled application instead of inserting a new row
                    existing.Status = "REVIEW";
                    existing.CurrentStep = 1;
                    existing.Message = message;
                    existing.CreatedAtUtc = DateTime.UtcNow;

                    await _db.SaveChangesAsync();

                    // Keep inventory consistent (should remain Available for REVIEW-only)
                    await RecomputePetInventoryStatusAsync(petId);

                    TempData["SuccessMessage"] = "Application re-submitted!";
                    return RedirectToAction(nameof(MyApplications));
                }

                TempData["ErrorMessage"] = "You already submitted an application for this pet.";
                return RedirectToAction(nameof(MyApplications));
            }

            // New application
            var app = new AdoptionApplication
            {
                UserId = userId,
                PetId = petId,
                Message = message,
                Status = "REVIEW",
                CurrentStep = 1,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.AdoptionApplications.Add(app);
            await _db.SaveChangesAsync();

            // Ensure inventory stays correct (REVIEW => Available)
            await RecomputePetInventoryStatusAsync(petId);

            TempData["SuccessMessage"] = "Application submitted!";
            return RedirectToAction(nameof(MyApplications));
        }


        [HttpGet]
        public async Task<IActionResult> MyApplications(string status = "All")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var userId = user.Id;

            // 1) Load ALL apps (including cancelled) for counts
            var allApps = await _db.AdoptionApplications
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .ToListAsync();

            // 2) Query for the cards (includes Pet)
            IQueryable<AdoptionApplication> appsQuery = _db.AdoptionApplications
                .AsNoTracking()
                .Include(a => a.Pet)
                .Where(a => a.UserId == userId);

            // 3) Apply filter rules
            var normalized = (status ?? "All").Trim().ToUpperInvariant();

            if (normalized == "ALL")
            {
                appsQuery = appsQuery.Where(a => (a.Status ?? "").ToUpper() != "CANCELLED");
            }
            else if (normalized == "CANCELLED" || normalized == "CANCELLED") // (second check optional)
            {
                appsQuery = appsQuery.Where(a => (a.Status ?? "").ToUpper() == "CANCELLED");
            }
            else
            {
                appsQuery = appsQuery.Where(a => (a.Status ?? "").ToUpper() == normalized);
            }

            var filteredApps = await appsQuery
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();

            ViewData["CurrentFilter"] = status;

            var vm = new ApplicationPageViewModel
            {
                UserName = GetDisplayName(user),

                Applications = filteredApps.Select(a => new ApplicationCardViewModel
                {
                    Id = a.Id,
                    ApplicationId = a.Id,       
                    PetId = a.PetId,

                    PetName = a.Pet?.Name ?? "Unknown",
                    Breed = a.Pet?.Breed ?? "",
                    Details = $"{a.Pet?.Species} • {a.Pet?.Age} years • {a.Pet?.Gender}",
                    ImageUrl = a.Pet?.ImageUrl ?? "",
                    Status = a.Status,
                    CurrentStep = StatusToStep(a.Status)

                    DateApplied = a.CreatedAtUtc.ToLocalTime().ToString("MMM dd, yyyy • h:mm tt"),
                    ApplicationMessage = a.Message ?? ""
                }).ToList(),

                Visits = new List<ScheduledVisitViewModel>(),
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
                "review" => 1,
                "pending" => 2,
                "approved" => 3,
                "scheduled" => 4,
                "completed" => 5,
                _ => 1
            };
        }

        private static Dictionary<string, int> BuildCounts(IEnumerable<AdoptionApplication> apps)
        {
            var list = apps.ToList();

            int cancelled = list.Count(a => string.Equals(a.Status, "CANCELLED", StringComparison.OrdinalIgnoreCase));
            int nonCancelled = list.Count - cancelled;

            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["All"] = nonCancelled,
                ["Review"] = list.Count(a => string.Equals(a.Status, "REVIEW", StringComparison.OrdinalIgnoreCase)),
                ["Pending"] = list.Count(a => string.Equals(a.Status, "PENDING", StringComparison.OrdinalIgnoreCase)),
                ["Approved"] = list.Count(a => string.Equals(a.Status, "APPROVED", StringComparison.OrdinalIgnoreCase)),
                ["Rejected"] = list.Count(a => string.Equals(a.Status, "REJECTED", StringComparison.OrdinalIgnoreCase)),
                ["Adopted"] = list.Count(a => string.Equals(a.Status, "ADOPTED", StringComparison.OrdinalIgnoreCase)),
                ["Cancelled"] = cancelled
            };
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageApplications(string status = "All")
        {
            // 1) Load ALL apps (including cancelled) for counts
            var allApps = await _db.AdoptionApplications
                .AsNoTracking()
                .ToListAsync();

            // 2) Query for the cards (includes Pet + User)
            IQueryable<AdoptionApplication> appsQuery = _db.AdoptionApplications
                .AsNoTracking()
                .Include(a => a.Pet)
                .Include(a => a.User);

            // 3) Apply filter rules
            var normalized = (status ?? "All").Trim().ToUpperInvariant();

            if (normalized == "ALL")
            {
                appsQuery = appsQuery.Where(a => (a.Status ?? "").ToUpper() != "CANCELLED");
            }
            else if (normalized == "CANCELLED" || normalized == "CANCELLED") // (second check optional)
            {
                appsQuery = appsQuery.Where(a => (a.Status ?? "").ToUpper() == "CANCELLED");
            }
            else
            {
                appsQuery = appsQuery.Where(a => (a.Status ?? "").ToUpper() == normalized);
            }

            var filteredApps = await appsQuery
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();

            var vm = new ApplicationPageViewModel
            {
                UserName = "Admin",

                Applications = filteredApps.Select(a =>
                {
                    var fullName = $"{a.User?.FirstName} {a.User?.LastName}".Trim();
                    if (string.IsNullOrWhiteSpace(fullName))
                    {
                        var candidate = a.User?.UserName ?? a.User?.Email ?? "Unknown";
                        fullName = candidate.Contains("@") ? candidate.Split('@')[0] : candidate;
                    }

                    return new ApplicationCardViewModel
                    {
                        Id = a.Id,
                        ApplicationId = a.Id,
                        PetId = a.PetId,

                        ApplicantName = fullName,
                        ApplicantEmail = a.User?.Email ?? "",

                        PetName = a.Pet?.Name ?? "Unknown",
                        Breed = a.Pet?.Breed ?? "",
                        Details = $"{a.Pet?.Species} • {a.Pet?.Age} years • {a.Pet?.Gender}",
                        ImageUrl = a.Pet?.ImageUrl ?? "",
                        Status = a.Status,
                        CurrentStep = a.CurrentStep

                        DateApplied = a.CreatedAtUtc.ToLocalTime().ToString("MMM dd, yyyy • h:mm tt"),
                        ApplicationMessage = a.Message ?? ""
                    };
                }).ToList(),

                Visits = new List<ScheduledVisitViewModel>(),
                StatusCounts = BuildCounts(allApps)
            };

            return View(vm);
        }


        // Cancel application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelApplication(int id)
        {
            Console.WriteLine(">>> CancelApplication HIT id=" + id);

            var app = await _db.AdoptionApplications.FirstOrDefaultAsync(a => a.Id == id);
            if (app == null) return NotFound("Application not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && app.UserId != userId)
                return Forbid();

            app.Status = "CANCELLED";
            await _db.SaveChangesAsync();

            await RecomputePetInventoryStatusAsync(app.PetId);

            return Ok();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateApplicationStatus(
        UpdateApplicationStatusRequest req,
        string status = "All" 
)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

                return RedirectToAction(nameof(ManageApplications), new { status });
            }

            var app = await _db.AdoptionApplications
                .Include(a => a.Pet)  
                .FirstOrDefaultAsync(a => a.Id == req.ApplicationId);

            if (app == null) return NotFound();

            // Normalize status
            var normalizedStatus = (req.Status ?? "").Trim().ToUpperInvariant();

            // Whitelist (important)
            var allowed = new HashSet<string> { "REVIEW", "PENDING", "REJECTED", "APPROVED", "CANCELLED", "ADOPTED" };
            if (!allowed.Contains(normalizedStatus))
            {
                TempData["Error"] = $"Invalid status value: {normalizedStatus}";
                return RedirectToAction(nameof(ManageApplications), new { status });
            }

            var step = Math.Clamp(req.CurrentStep, 1, 4);

            app.Status = normalizedStatus;
            app.CurrentStep = step;

            // Save app changes first
            await _db.SaveChangesAsync();

            // Now recompute inventory based on ALL applications for that pet
            await RecomputePetInventoryStatusAsync(app.PetId);

            TempData["Success"] = "Application updated.";
            return RedirectToAction(nameof(ManageApplications), new { status });
        }
    }
}