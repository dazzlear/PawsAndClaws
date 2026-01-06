using Microsoft.AspNetCore.Mvc;
using PawsAndClaws.Models;

namespace PawsAndClaws.Controllers
{
    public class AdoptionController : Controller
    {
        public IActionResult MyApplications(string status = "All")
        {
            var allApplications = new List<AdoptionApplication>
            {
                new AdoptionApplication
                {
                    PetName = "Luna",
                    Breed = "Golden Retriever",
                    Details = "2 Years • Female",
                    ImageUrl = "https://images.unsplash.com/photo-1552053831-71594a27632d?auto=format&fit=crop&q=80&w=500",
                    Status = "APPROVED",
                    CurrentStep = 3
                },
                new AdoptionApplication
                {
                    PetName = "Oliver",
                    Breed = "Tabby Cat",
                    Details = "6 Months • Male",
                    ImageUrl = "https://images.unsplash.com/photo-1514888286974-6c03e2ca1dba?auto=format&fit=crop&q=80&w=500",
                    Status = "PENDING",
                    CurrentStep = 1
                },
                new AdoptionApplication
                {
                    PetName = "Bella",
                    Breed = "Beagle",
                    Details = "1 Year • Female",
                    ImageUrl = "https://images.unsplash.com/photo-1537151608828-ea2b11777ee8?auto=format&fit=crop&q=80&w=500",
                    Status = "APPROVED",
                    CurrentStep = 4
                },
                new AdoptionApplication
                {
                    PetName = "Charlie",
                    Breed = "Labrador",
                    Details = "3 Years • Male",
                    ImageUrl = "https://images.unsplash.com/photo-1583511655857-d19b40a7a54e?auto=format&fit=crop&q=80&w=500",
                    Status = "REJECTED",
                    CurrentStep = 2
                }
            };

            var filteredApplications = status == "All" 
                ? allApplications 
                : allApplications.Where(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

            var viewModel = new ApplicationPageViewModel
            {
                UserName = "Julianna",
                Applications = filteredApplications,
                Visits = new List<ScheduledVisit>
                {
                    new ScheduledVisit
                    {
                        PetName = "Luna",
                        Location = "Main Shelter",
                        Schedule = DateTime.Now.AddDays(2)
                    }
                },
                StatusCounts = new Dictionary<string, int>
                {
                    { "All", allApplications.Count },
                    { "Approved", allApplications.Count(a => a.Status == "APPROVED") },
                    { "Pending", allApplications.Count(a => a.Status == "PENDING") },
                    { "Rejected", allApplications.Count(a => a.Status == "REJECTED") }
                }
            };

            ViewData["CurrentFilter"] = status;

            return View(viewModel);
        }
    }
}
