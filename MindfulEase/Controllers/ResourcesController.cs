using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Services.MindfulEase.Services;

namespace MindfulEase.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResourcesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteModerator = User.IsInRole("Moderator");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult New()
        {
            return View();
        }


        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult New(Resource resource)
        {
            if (ModelState.IsValid)
            {
                db.Resources.Add(resource);
                // Salvează modificările în baza de date
                db.SaveChanges();
                TempData["message"] = "Your resource has been created successfully!";
                TempData["messageType"] = "alert-success";
            }

           

            TempData["message"] = "Your resource has not been created!";
            TempData["messageType"] = "alert-danger";

            return Redirect("/Routines/Index");

        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Resource resource = db.Resources
                                        .Where(u => u.Id == id)
                                        .First();
            return View(resource);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, Resource requestResource)
        {
            Resource resource = db.Resources.Find(id);

            resource.Title = requestResource.Title;
            resource.Link =  requestResource.Link;
            resource.ResourceType = requestResource.ResourceType;

            TempData["message"] = "You edited your resource successfully!";
            TempData["messageType"] = "alert-success";
            db.SaveChanges();

            return Redirect("/Routines/Index");
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var resource = db.Resources.FirstOrDefault(d => d.Id == id);
            if (resource == null)
            {
                TempData["message"] = "Resource not found or you do not have permission to delete it.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("New");
            }

            db.Resources.Remove(resource);
            db.SaveChanges();

            TempData["message"] = "Resource deleted successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("New");
        }

    }
}
