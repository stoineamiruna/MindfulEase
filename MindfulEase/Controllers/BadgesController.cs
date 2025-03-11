using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Services;

namespace MindfulEase.Controllers
{
    public class BadgesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly BadgeService _badgeService;

        public BadgesController(ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager,
           BadgeService badgeService)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _badgeService = badgeService;
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
        public IActionResult Index()
        {
            SetAccessRights();
            var badges = db.Badges.ToList();
            return View(badges);
        }

        [Authorize(Roles = "Admin,Moderator,User")]
        [Route("Badges/IndexUserBadges/{userId}")]
        public IActionResult IndexUserBadges(string userId)
        {
            SetAccessRights();

            // Verificăm dacă utilizatorul există
            var user = db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            // Obținem badge-urile utilizatorului
            var userBadges = db.UserBadges
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.Badge)
                .ToList();

            // Returnăm view-ul badge-urilor utilizatorului
            ViewBag.User = user;
            return View(userBadges);
        }


        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult New(Badge badge)
        {
            if (ModelState.IsValid)
            {
                db.Badges.Add(badge);
                db.SaveChanges();
                TempData["message"] = "Badge created successfully!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            TempData["message"] = "There was an error creating the badge.";
            TempData["messageType"] = "alert-danger";
            return View(badge);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Edit(int id)
        {
            var badge = db.Badges.FirstOrDefault(q => q.Id == id);
            if (badge == null)
            {
                TempData["message"] = "Badge not found.";
                TempData["messageType"] = "alert-danger";
                return NotFound();
            }
            return View(badge);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, Badge updatedBadge)
        {
            var badge = db.Badges.FirstOrDefault(b => b.Id == id);
            if (badge == null)
            {
                TempData["message"] = "Badge not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            badge.Title = updatedBadge.Title;
            badge.Description = updatedBadge.Description;
            badge.ImageUrl = updatedBadge.ImageUrl;
            badge.Activity = updatedBadge.Activity;
            db.SaveChanges();
            TempData["message"] = "Badge updated successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var badge = db.Badges.Find(id);
            if (badge == null)
            {
                TempData["message"] = "Badge not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            if (badge.Users != null)
            {
                db.UserBadges.RemoveRange(badge.Users);
            }
            db.Badges.Remove(badge);
            db.SaveChanges();
            TempData["message"] = "Badge deleted successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }
    
    }
}
