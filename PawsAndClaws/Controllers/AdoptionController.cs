using Microsoft.AspNetCore.Mvc;

namespace PawsAndClaws.Controllers
{
    public class AdoptionController : Controller
    {
        public IActionResult MyApplications()
        {
            return Content("My Applications Placeholder");
        }
    }
}
