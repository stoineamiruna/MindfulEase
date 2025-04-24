using Microsoft.AspNetCore.Mvc;

namespace MindfulEase.Controllers
{
    public class VisualizeController : Controller
    {
        public IActionResult Brain()
        {
            return View();
        }
    }
}
