using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PawsAndClaws.Models;

namespace PawsAndClaws.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        public IActionResult Index()
        {
            var pets = new List<Pet>
            {
                new Pet { Name = "Megatron", Breed = "Aspin", Age = 2, Gender = "Male", Location = "Quezon City, PH", Status = "Pending", ImageUrl = "/images/Megatron.jpg" },
                new Pet { Name = "Asteroid Destroyer", Breed = "Persian", Age = 1, Gender = "Female", Location = "Quezon City, PH", Status = "Available", ImageUrl = "/images/Asteroid.jpg" },
                new Pet { Name = "Josie Megatron", Breed = "Persian", Age = 1, Gender = "Female", Location = "Quezon City, PH", Status = "Available", ImageUrl = "/images/Josie.png" },
                new Pet { Name = "Mini Melay", Breed = "Chihuahua", Age = 1, Gender = "Male", Location = "Quezon City, PH", Status = "Available", ImageUrl = "/images/Melay.jpg" },
                new Pet { Name = "Marites", Breed = "Persian", Age = 3, Gender = "Female", Location = "Taguig City, PH", Status = "Available", ImageUrl = "/images/Marites.png" },
                new Pet { Name = "Monami", Breed = "Aspin", Age = 3, Gender = "Female", Location = "Quezon City, PH", Status = "Pending", ImageUrl = "/images/Monami.jpg" },
                new Pet { Name = "Greg Yapper", Breed = "British Shorthair", Age = 1, Gender = "Male", Location = "Quezon City, PH", Status = "Available", ImageUrl = "/images/Greg.png" },
                new Pet { Name = "Galaxy Annihilator", Breed = "Persian", Age = 1, Gender = "Female", Location = "Quezon City, PH", Status = "Pending", ImageUrl = "/images/Galaxy.jpg" }
            };
            return View(pets);
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
    }
}
