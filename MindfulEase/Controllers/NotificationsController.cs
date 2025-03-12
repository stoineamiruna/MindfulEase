using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindfulEase.Data;
using MindfulEase.Models;

namespace MindfulEase.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public NotificationsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Afișarea notificărilor utilizatorului curent (personale și globale)
        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User);

            var notifications = _db.Notifications
                .Where(n => (n.UserId == null || n.UserId == userId) && n.IsRead == false) // Notificări globale sau specifice
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            ViewBag.Message = TempData["message"];
            ViewBag.MessageType = TempData["messageType"];

            return View(notifications);
        }

        // Marcare notificare ca citită
        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            var notification = _db.Notifications.Find(id);

            if (notification != null && (notification.UserId == null || notification.UserId == _userManager.GetUserId(User)))
            {
                notification.IsRead = true;
                _db.SaveChanges();

                TempData["message"] = "Notification marked as read.";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Unable to mark notification as read.";
                TempData["messageType"] = "alert-danger";
            }

            return RedirectToAction("Index");
        }

        // Adăugarea unei notificări noi
        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult New(Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.CreatedAt = DateTime.Now;

                // Dacă UserId este null, notificarea va fi globală
                _db.Notifications.Add(notification);
                _db.SaveChanges();

                TempData["message"] = "Notification added successfully.";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Index");
            }

            return View(notification);
        }

        // Ștergerea unei notificări
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var notification = _db.Notifications.Find(id);

            if (notification != null)
            {
                _db.Notifications.Remove(notification);
                _db.SaveChanges();

                TempData["message"] = "Notification deleted successfully.";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Unable to delete notification.";
                TempData["messageType"] = "alert-danger";
            }

            return RedirectToAction("Index");
        }

        // Vizualizarea unei notificări specifice
        public IActionResult Show(int id)
        {
            var notification = _db.Notifications.Find(id);

            if (notification == null || (notification.UserId != null && notification.UserId != _userManager.GetUserId(User)))
            {
                return NotFound();
            }

            return View(notification);
        }
    }
}
