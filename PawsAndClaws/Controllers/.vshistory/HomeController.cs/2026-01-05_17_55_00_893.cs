using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PawsAndClaws.Models.ViewModels;

namespace PawsAndClaws.Controllers
{
    public class HomeController : Controller
    {
        // 1. MAIN LANDING PAGE
        public IActionResult Index()
        {
            var pets = GetSamplePets();
            return View(pets);
        }

        // 2. PET DETAILS / PROFILE VIEW
        // This is triggered when a user clicks a pet card
        public IActionResult Details(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index");
            }

            // Find the specific pet from our sample list
            var pet = GetSamplePets().FirstOrDefault(p => p.Name == name);

            if (pet == null)
            {
                return NotFound();
            }

            // Standardize Description for the profile view (as seen in image_f5683e.jpg)
            pet.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

            return View(pet);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // 3. SAMPLE DATA SOURCE
        // Centralized list used by both Index and Details
        private List<Pet> GetSamplePets()
        {
            return new List<Pet>
            {
                new Pet
                {
                    Name = "Megatron",
                    Breed = "Aspin",
                    Species = "Dog",
                    Age = 2,
                    Gender = "Male",
                    Size = "Medium",
                    Location = "Quezon City, PH",
                    Status = "Pending",
                    ImageUrl = "/images/Megatron.jpg"
                },
                new Pet
                {
                    Name = "Asteroid Destroyer",
                    Breed = "Persian",
                    Species = "Cat",
                    Age = 1,
                    Gender = "Female",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Asteroid.jpg"
                },
                new Pet
                {
                    Name = "Josie Megatron",
                    Breed = "Persian",
                    Species = "Cat",
                    Age = 1,
                    Gender = "Female",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Josie.png"
                },
                new Pet
                {
                    Name = "Mini Melay",
                    Breed = "Chihuahua",
                    Species = "Dog",
                    Age = 1,
                    Gender = "Male",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Melay.jpg"
                },
                new Pet
                {
                    Name = "Marites",
                    Breed = "Persian",
                    Species = "Cat",
                    Age = 3,
                    Gender = "Female",
                    Size = "Small",
                    Location = "Taguig City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Marites.png"
                },
                new Pet
                {
                    Name = "Monami",
                    Breed = "Aspin",
                    Species = "Dog",
                    Age = 3,
                    Gender = "Female",
                    Size = "Medium",
                    Location = "Quezon City, PH",
                    Status = "Pending",
                    ImageUrl = "/images/Monami.jpg"
                },
                new Pet
                {
                    Name = "Greg Yapper",
                    Breed = "British Shorthair",
                    Species = "Cat",
                    Age = 1,
                    Gender = "Male",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Available",
                    ImageUrl = "/images/Greg.png"
                },
                new Pet
                {
                    Name = "Galaxy Annihilator",
                    Breed = "Persian",
                    Species = "Cat",
                    Age = 1,
                    Gender = "Female",
                    Size = "Small",
                    Location = "Quezon City, PH",
                    Status = "Pending",
                    ImageUrl = "/images/Galaxy.jpg"
                }
            };
        }
    }
}