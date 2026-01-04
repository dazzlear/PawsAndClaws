using Microsoft.AspNetCore.Mvc;
using PawsAndClaws.Models;
using System.Text.Json; // Needed for saving data to TempData
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace PawsAndClaws.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Let me handle the login logic here
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Email, model.Email),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            return Content("Profile Placeholder");
        }

        // STEP 1: Show the Account Registration Page
        [HttpGet]
        public IActionResult Register()
        {
            var step1Json = TempData["UserStep1"] as string;
            if (!string.IsNullOrEmpty(step1Json))
            {
                TempData.Keep("UserStep1");
                var model = JsonSerializer.Deserialize<RegisterViewModel>(step1Json);
                return View(model);
            }
            return View();
        }

        // STEP 1 POST: Receive Account Info and Move to Step 2
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Temporarily store the first form's data as a JSON string
                TempData["UserStep1"] = JsonSerializer.Serialize(model);
                return RedirectToAction("RegisterStep2");
            }
            return View(model);
        }

        // STEP 2: Show the Lifestyle Question Page
        [HttpGet]
        public IActionResult RegisterStep2()
        {
            TempData.Keep("UserStep1");
            var hasOtherPets = TempData["HasOtherPets"] as bool?;
            if (hasOtherPets.HasValue)
            {
                TempData.Keep("HasOtherPets");
                return View(new PetPreferenceViewModel { HasOtherPets = hasOtherPets.Value });
            }
            return View(new PetPreferenceViewModel { HasOtherPets = true }); // Default to true
        }

        // STEP 2 POST: Decide whether to go to Step 3 or Step 4
        [HttpPost]
        public IActionResult ProcessStep2(bool hasOtherPets)
        {
            // Retrieve the data we saved from Step 1
            var step1Json = TempData["UserStep1"] as string;
            
            if (string.IsNullOrEmpty(step1Json))
            {
                return RedirectToAction("Register"); // Safety check: go back if data is lost
            }

            // Keep TempData for the next steps
            TempData.Keep("UserStep1");
            TempData["HasOtherPets"] = hasOtherPets;

            if (hasOtherPets)
            {
                return RedirectToAction("RegisterStep3");
            }
            else
            {
                // If they changed from "Yes" to "No", clear any pets they might have added
                TempData.Remove("UserPets");
                return RedirectToAction("RegisterStep4");
            }
        }

        [HttpGet]
        public IActionResult RegisterStep3()
        {
            var petsJson = TempData["UserPets"] as string;
            var pets = string.IsNullOrEmpty(petsJson) 
                ? new List<AddPetViewModel>() 
                : JsonSerializer.Deserialize<List<AddPetViewModel>>(petsJson) ?? new List<AddPetViewModel>();
            
            TempData.Keep("UserStep1"); // Keep step 1 data
            TempData.Keep("UserPets");
            TempData.Keep("HasOtherPets");
            ViewBag.Pets = pets;
            return View(new AddPetViewModel());
        }

        [HttpPost]
        public IActionResult ProcessStep3(AddPetViewModel model, string action)
        {
            var petsJson = TempData["UserPets"] as string;
            var pets = string.IsNullOrEmpty(petsJson) 
                ? new List<AddPetViewModel>() 
                : JsonSerializer.Deserialize<List<AddPetViewModel>>(petsJson) ?? new List<AddPetViewModel>();

            if (action == "AddPet")
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(model.Breed))
                {
                    pets.Add(model);
                    TempData["UserPets"] = JsonSerializer.Serialize(pets);
                    TempData.Keep("UserStep1");
                    TempData.Keep("HasOtherPets");
                    return RedirectToAction("RegisterStep3");
                }
            }
            else if (action != null && action.StartsWith("RemovePet:"))
            {
                if (int.TryParse(action.Split(':')[1], out int index))
                {
                    if (index >= 0 && index < pets.Count)
                    {
                        pets.RemoveAt(index);
                        TempData["UserPets"] = JsonSerializer.Serialize(pets);
                        TempData.Keep("UserStep1");
                        TempData.Keep("HasOtherPets");
                        return RedirectToAction("RegisterStep3");
                    }
                }
            }
            else if (action == "Proceed")
            {
                TempData["UserPets"] = JsonSerializer.Serialize(pets);
                TempData.Keep("UserStep1");
                TempData.Keep("HasOtherPets");
                return RedirectToAction("RegisterStep4");
            }

            TempData.Keep("UserStep1");
            TempData.Keep("UserPets");
            TempData.Keep("HasOtherPets");
            ViewBag.Pets = pets;
            return View("RegisterStep3", model);
        }

        [HttpGet]
        public IActionResult RegisterStep4()
        {
            TempData.Keep("UserStep1");
            var petsJson = TempData["UserPets"] as string;
            TempData.Keep("UserPets");
            TempData.Keep("HasOtherPets");

            // Determine where the "Back" button should go
            ViewBag.BackAction = (string.IsNullOrEmpty(petsJson) || petsJson == "[]") ? "RegisterStep2" : "RegisterStep3";

            var homeJson = TempData["UserHome"] as string;
            if (!string.IsNullOrEmpty(homeJson))
            {
                TempData.Keep("UserHome");
                var model = JsonSerializer.Deserialize<HomeLivingViewModel>(homeJson);
                return View(model);
            }

            return View(new HomeLivingViewModel { Address = "", LivingSituation = "House" });
        }

        [HttpPost]
        public IActionResult CompleteRegistration(HomeLivingViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve all data
                var step1Json = TempData["UserStep1"] as string;
                var petsJson = TempData["UserPets"] as string;

                // LOGIC: Save everything to DB
                
                // Clear TempData after successful registration
                TempData.Remove("UserStep1");
                TempData.Remove("UserPets");
                TempData.Remove("HasOtherPets");
                TempData.Remove("UserHome");

                return RedirectToAction("Index", "Home");
            }

            // If we got here, something failed. Re-set the BackAction for the view.
            TempData.Keep("UserStep1");
            var petsJsonForBack = TempData["UserPets"] as string;
            TempData.Keep("UserPets");
            TempData.Keep("HasOtherPets");
            
            // Save current progress even if invalid (optional, but helps if they navigate away)
            TempData["UserHome"] = JsonSerializer.Serialize(model);

            ViewBag.BackAction = (string.IsNullOrEmpty(petsJsonForBack) || petsJsonForBack == "[]") ? "RegisterStep2" : "RegisterStep3";

            return View("RegisterStep4", model);
        }   
    }
}