using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PawsAndClaws.Data;
using PawsAndClaws.Models;
using PawsAndClaws.Models.Identity;
using PawsAndClaws.Models.ViewModels;
using System.Text.Json;

namespace PawsAndClaws.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            AppDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }

        // -------------------------
        // LOGIN / LOGOUT (DB-backed)

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // Ensure clean session (optional)
            await _signInManager.SignOutAsync();

            var result = await _signInManager.PasswordSignInAsync(
                userName: model.Email,
                password: model.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            return Content("Profile Placeholder");
        }

        // REGISTER (TempData Wizard)

        // STEP 1 GET
        [HttpGet]
        public IActionResult Register()
        {
            var step1 = GetFromTempData<RegisterViewModel>("UserStep1");
            if (step1 != null)
            {
                TempData.Keep("UserStep1");
                return View(step1);
            }

            return View();
        }

        // STEP 1 POST -> STEP 2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            TempData["UserStep1"] = JsonSerializer.Serialize(model, _jsonOptions);
            return RedirectToAction("RegisterStep2");
        }

        // STEP 2 GET
        [HttpGet]
        public IActionResult RegisterStep2()
        {
            TempData.Keep("UserStep1");

            var hasOtherPets = GetBoolFromTempData("HasOtherPets", defaultValue: true);
            TempData.Keep("HasOtherPets");

            return View(new PetPreferenceViewModel { HasOtherPets = hasOtherPets });
        }

        // STEP 2 POST -> STEP 3 or STEP 4
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessStep2(bool hasOtherPets)
        {
            TempData.Keep("UserStep1");
            TempData["HasOtherPets"] = hasOtherPets;

            if (hasOtherPets)
            {
                return RedirectToAction("RegisterStep3");
            }

            // If user chose "No", clear any previously added pets
            TempData.Remove("UserPets");
            return RedirectToAction("RegisterStep4");
        }

        // STEP 3 GET (list pets + add form)
        [HttpGet]
        public IActionResult RegisterStep3()
        {
            var pets = GetPetsFromTempData();

            TempData.Keep("UserStep1");
            TempData.Keep("UserPets");
            TempData.Keep("HasOtherPets");

            ViewBag.Pets = pets;
            return View(new AddPetViewModel());
        }

        // STEP 3 POST (Add / Remove / Proceed)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessStep3(AddPetViewModel model, string action)
        {
            var pets = GetPetsFromTempData();

            if (action == "AddPet")
            {
                if (!ModelState.IsValid)
                {
                    TempData.Keep("UserStep1");
                    TempData.Keep("HasOtherPets");
                    TempData.Keep("UserPets");

                    ViewBag.Pets = pets;
                    return View("RegisterStep3", model);
                }

                pets.Add(model);
                TempData["UserPets"] = JsonSerializer.Serialize(pets, _jsonOptions);

                TempData.Keep("UserStep1");
                TempData.Keep("HasOtherPets");
                return RedirectToAction("RegisterStep3");
            }
            else if (!string.IsNullOrEmpty(action) && action.StartsWith("RemovePet:"))
            {
                if (int.TryParse(action.Split(':')[1], out int index))
                {
                    if (index >= 0 && index < pets.Count)
                    {
                        pets.RemoveAt(index);
                        TempData["UserPets"] = JsonSerializer.Serialize(pets, _jsonOptions);
                    }
                }

                TempData.Keep("UserStep1");
                TempData.Keep("HasOtherPets");
                return RedirectToAction("RegisterStep3");
            }
            else if (action == "Proceed")
            {
                TempData["UserPets"] = JsonSerializer.Serialize(pets, _jsonOptions);

                TempData.Keep("UserStep1");
                TempData.Keep("HasOtherPets");
                return RedirectToAction("RegisterStep4");
            }

            TempData.Keep("UserStep1");
            TempData.Keep("HasOtherPets");
            TempData.Keep("UserPets");
            return RedirectToAction("RegisterStep3");
        }

        // STEP 4 GET
        [HttpGet]
        public IActionResult RegisterStep4()
        {
            TempData.Keep("UserStep1");
            TempData.Keep("UserPets");
            TempData.Keep("HasOtherPets");

            var petsJson = TempData.Peek("UserPets") as string;
            ViewBag.BackAction = (string.IsNullOrEmpty(petsJson) || petsJson == "[]")
                ? "RegisterStep2"
                : "RegisterStep3";

            ViewBag.Pets = GetPetsFromTempData();

            var home = GetFromTempData<HomeLivingViewModel>("UserHome");
            if (home != null)
            {
                TempData.Keep("UserHome");
                return View(home);
            }

            return View(new HomeLivingViewModel { Address = "", LivingSituation = "House" });
        }

        // STEP 4 POST -> SAVE TO DB + SIGN IN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteRegistration(HomeLivingViewModel model)
        {
            // Keep wizard data alive while validating/saving
            TempData.Keep("UserStep1");
            TempData.Keep("UserPets");
            TempData.Keep("HasOtherPets");

            var petsJsonForBack = TempData.Peek("UserPets") as string;
            ViewBag.BackAction = (string.IsNullOrEmpty(petsJsonForBack) || petsJsonForBack == "[]")
                ? "RegisterStep2"
                : "RegisterStep3";

            ViewBag.Pets = GetPetsFromTempData();

            if (!ModelState.IsValid)
            {
                // Persist home step if validation failed
                TempData["UserHome"] = JsonSerializer.Serialize(model, _jsonOptions);
                TempData.Keep("UserHome");
                return View("RegisterStep4", model);
            }

            // Retrieve all data from Step 1
            var step1 = GetFromTempData<RegisterViewModel>("UserStep1");
            if (step1 == null)
            {
                // If user refreshed/expired TempData, restart wizard safely
                return RedirectToAction("Register");
            }

            var hasOtherPets = GetBoolFromTempData("HasOtherPets", defaultValue: false);
            var petsJson = TempData.Peek("UserPets") as string ?? "[]";

            // Check if email already exists
            var existing = await _userManager.FindByEmailAsync(step1.Email);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(RegisterViewModel.Email), "Email is already registered.");
                TempData["UserHome"] = JsonSerializer.Serialize(model, _jsonOptions);
                TempData.Keep("UserHome");
                return View("RegisterStep4", model);
            }

            // Create user (DB)
            var user = new ApplicationUser
            {
                UserName = step1.Email,
                Email = step1.Email,

                // from Step 1
                FirstName = step1.FirstName,
                LastName = step1.LastName,

                // from Step 2
                HasOtherPets = hasOtherPets,

                // from Step 4
                Address = model.Address ?? "",
                LivingSituation = model.LivingSituation ?? "House",

                // store Step 3 list (JSON) in DB column
                OtherPetsJson = hasOtherPets ? petsJson : "[]"
            };

            var createResult = await _userManager.CreateAsync(user, step1.Password);
            if (!createResult.Succeeded)
            {
                foreach (var err in createResult.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                TempData["UserHome"] = JsonSerializer.Serialize(model, _jsonOptions);
                TempData.Keep("UserHome");
                return View("RegisterStep4", model);
            }

            // Sign in (DB-backed cookie via Identity)
            await _signInManager.SignInAsync(user, isPersistent: true);

            // Clear TempData after successful registration
            TempData.Remove("UserStep1");
            TempData.Remove("UserPets");
            TempData.Remove("HasOtherPets");
            TempData.Remove("UserHome");

            return RedirectToAction("Index", "Home");
        }

        // -------------------------
        // Helpers
        // -------------------------

        private T? GetFromTempData<T>(string key)
        {
            // Peek doesn't mark for deletion; safer in multi-step flows
            var raw = TempData.Peek(key);

            if (raw is T direct)
                return direct;

            if (raw is string s && !string.IsNullOrWhiteSpace(s))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(s, _jsonOptions);
                }
                catch
                {
                    return default;
                }
            }

            return default;
        }

        private bool GetBoolFromTempData(string key, bool defaultValue)
        {
            var raw = TempData.Peek(key);

            if (raw is bool b) return b;

            if (raw is string s && bool.TryParse(s, out var parsed))
                return parsed;

            return defaultValue;
        }

        private List<AddPetViewModel> GetPetsFromTempData()
        {
            var petsJson = TempData.Peek("UserPets") as string;

            if (string.IsNullOrWhiteSpace(petsJson))
                return new List<AddPetViewModel>();

            try
            {
                return JsonSerializer.Deserialize<List<AddPetViewModel>>(petsJson, _jsonOptions)
                       ?? new List<AddPetViewModel>();
            }
            catch
            {
                return new List<AddPetViewModel>();
            }
        }
    }
}