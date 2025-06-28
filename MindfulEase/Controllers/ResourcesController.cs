using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
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
            ViewBag.Tags = db.Tags.ToList();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }


        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult New(Resource resource, List<int> SelectedTags)
        {
            string referer = Request.Headers["Referer"].ToString();
            if (ModelState.IsValid)
            {
                db.Resources.Add(resource);
                // Salvează modificările în baza de date
                db.SaveChanges();

                // Asociază tagurile selectate
                if (SelectedTags != null && SelectedTags.Any())
                {
                    foreach (var tagId in SelectedTags)
                    {
                        db.ResourceTags.Add(new ResourceTag { ResourceId = resource.Id, TagId = tagId });
                    }
                    db.SaveChanges();
                }

                TempData["message"] = "Your resource has been created successfully!";
                TempData["messageType"] = "alert-info";
                
                return RedirectToAction("Index", "Routines");
            }

            // Dacă avem erori de validare, ne întoarcem în view
            ViewBag.Tags = db.Tags.ToList();
            return View(resource);
            /*
            TempData["message"] = "Your resource has not been created!";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index", "Routines");*/

        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Resource resource = db.Resources
                                        .Where(u => u.Id == id)
                                        .First();
            ViewBag.Tags = db.Tags.ToList(); // Toate tagurile disponibile

            var selectedTags = db.ResourceTags
                      .Where(rt => rt.ResourceId == id) // Filtrează după ResourceId
                      .Select(rt => rt.TagId) // Selectează doar ID-urile tagurilor
                      .Where(tagId => tagId.HasValue) // Filtrează doar tagurile care nu sunt null
                      .Select(tagId => tagId.Value) // Extrage valoarea de tip `int`
                      .ToList();

            ViewBag.SelectedTags = selectedTags;


            Console.WriteLine("selected tags: "+selectedTags.Count);
            return View(resource);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, Resource requestResource, List<int> SelectedTags)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tags = db.Tags.ToList();
                ViewBag.SelectedTags = SelectedTags;
                return View(requestResource); // trimite modelul înapoi pentru validare
            }
            var resource = db.Resources.Find(id);

            resource.Title = requestResource.Title;
            resource.Link =  requestResource.Link;
            resource.ResourceType = requestResource.ResourceType;

            // Asigură-te că Tags nu este null
            if (resource.Tags == null)
            {
                resource.Tags = new List<ResourceTag>();
            }

            // Șterge toate tagurile asociate anterior (din tabela ResourceTags)
            var existingTags = db.ResourceTags.Where(rt => rt.ResourceId == resource.Id).ToList();
            db.ResourceTags.RemoveRange(existingTags);

            // Adaugă tagurile noi
            if (SelectedTags != null && SelectedTags.Any())
            {
                foreach (var tagId in SelectedTags)
                {
                    db.ResourceTags.Add(new ResourceTag { ResourceId = resource.Id, TagId = tagId });
                }
            }

            TempData["message"] = "You edited your resource successfully!";
            TempData["messageType"] = "alert-info";
            db.SaveChanges();
            return RedirectToAction("Index", "Routines");
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
            // Ștergem toate intrările asociate din SavedResources
            var savedEntries = db.SavedResources.Where(sr => sr.ResourceId == id).ToList();
            db.SavedResources.RemoveRange(savedEntries);

            // Ștergem toate intrările asociate din SavedResources
            var likedEntries = db.ApplicationUserResources.Where(sr => sr.ResourceId == id).ToList();
            db.ApplicationUserResources.RemoveRange(likedEntries);

            var resourceTags = db.ResourceTags.Where(sr => sr.ResourceId == id).ToList();
            db.ResourceTags.RemoveRange(resourceTags);

            db.Resources.Remove(resource);
            db.SaveChanges();

            TempData["message"] = "Resource deleted successfully!";
            TempData["messageType"] = "alert-info";
            return RedirectToAction("Index", "Routines"); 

        }

    }
}
