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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int petId, string message)
        {
            message = (message ?? "").Trim();

            // 1) Validate message length
            if (message.Length < 20)
            {
                TempData["ErrorMessage"] = "Please provide a bit more detail (at least 20 characters).";
                return RedirectToAction("Details", "Home", new { id = petId });
            }

            // 2) Get user
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            // 3) Ensure pet exists
            var petExists = await _db.Pets.AsNoTracking().AnyAsync(p => p.Id == petId);
            if (!petExists)
            {
                TempData["ErrorMessage"] = "Pet not found.";
                return RedirectToAction("Index", "Home");
            }

            // 4) Prevent duplicate application (ANY status)
            var alreadyApplied = await _db.AdoptionApplications
                .AnyAsync(a => a.UserId == userId
                            && a.PetId == petId
                            && a.Status != "CANCELLED");

            if (alreadyApplied)
            {
                TempData["ErrorMessage"] = "You already submitted an application for this pet.";
                return RedirectToAction(nameof(MyApplications));
            }

            // 5) Save new application
            var app = new AdoptionApplication
            {
                UserId = userId,
                PetId = petId,
                Message = message,
                Status = "REVIEW",
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
                .FirstOrDefaultAsync(a => a.Id == req.ApplicationId);

            if (app == null) return NotFound();

            // Normalize status
            var normalizedStatus = (req.Status ?? "").Trim().ToUpperInvariant();

            // Whitelist (important)
            var allowed = new HashSet<string> { "REVIEW", "PENDING", "REJECTED", "APPROVED", "CANCELLED" };
            if (!allowed.Contains(normalizedStatus))
            {
                TempData["Error"] = $"Invalid status value: {normalizedStatus}";
                return RedirectToAction(nameof(ManageApplications), new { status });
            }

            // Clamp step (avoid 0 or >4)
            var step = Math.Clamp(req.CurrentStep, 1, 4);

            app.Status = normalizedStatus;
            app.CurrentStep = step;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Application updated.";

            return RedirectToAction(nameof(ManageApplications), new { status });
        }
    }
}