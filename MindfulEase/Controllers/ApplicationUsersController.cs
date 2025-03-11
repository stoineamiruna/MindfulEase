using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Services;
using System;

namespace MindfulEase.Controllers
{
    public class ApplicationUsersController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RewardService _rewardService;


        public ApplicationUsersController(ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager,
           RewardService rewardService)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _rewardService = rewardService;
        }

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteModerator = User.IsInRole("Moderator");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }


        // lista tututor userilor si search bar
        [Authorize(Roles = "Admin,Moderator,User")]
        public IActionResult Index(string search)
        {
            SetAccessRights();
            var users = db.ApplicationUsers.Where(u => !string.IsNullOrEmpty(u.FirstName) && !string.IsNullOrEmpty(u.LastName) && u.IsTherapist!=null); // Convertim DbSet în interogare

            if (!string.IsNullOrEmpty(search))
            {
                // Cautăm după nume sau prenume
                users = users.Where(a => a.FirstName.Contains(search) || a.LastName.Contains(search));
            }

            ViewBag.Users = users.ToList(); // Obținem lista completă de utilizatori după aplicarea filtrului
            ViewBag.SearchString = search;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }




        //afisarea unui singur profil in functie de id-ul sau
        [Authorize(Roles = "Admin,Moderator,User")]
        public async Task<IActionResult> Show(string? id)
        {
            SetAccessRights();

            if (!User.Identity.IsAuthenticated)
            {
                // Dacă utilizatorul nu este autentificat, direcționează-l către pagina de înregistrare
                return Redirect("/Identity/Account/Login");
            }

            //ApplicationUser currentUser = _userManager.GetUserAsync(User).Result;

            ApplicationUser user = db.ApplicationUsers
                          .Where(u => u.Id == id)
                          .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }
            ViewBag.User = user;

            // Obținem obiectivele utilizatorului(din UserObjective, legate de Objective)
            var userObjectives = db.UserObjectives
                .Where(uo => uo.UserId == id)
                .Include(uo => uo.Objective) // Încărcăm și informațiile despre Objective
                .ToList();

            ViewBag.Points = await _rewardService.GetTotalPointsAsync(id);
            ViewBag.UserObjectives = userObjectives;
            
            // Obținem badge-urile utilizatorului și limităm la 7
            var userBadges = db.UserBadges
                .Where(ub => ub.UserId == id)
                .Select(ub => ub.Badge)
                .Take(7) // Limitează numărul de badge-uri la 7
                .ToList();


            ViewBag.UserBadges = userBadges;

            ViewBag.DaysStreak = CalculateDaysStreak(id);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }
        private int CalculateDaysStreak(string userId)
        {
            var userObjectives = db.UserObjectives
                            .Where(uo => uo.UserId == userId)
                            .Include(uo => uo.Objective)
                            .ToList();

            // Obținem Id-urile obiectivelor utilizatorului
            var userObjectiveIds = userObjectives.Select(uo => (int?)uo.Id).ToList();

            var userObjectiveProgresses = db.UserObjectiveProgresses
                .Where(up => userObjectiveIds.Contains(up.UserObjectiveId) && up.IsCompleted)
                .OrderByDescending(up => up.Date) // Sortăm descrescător pentru a verifica zile consecutive
                .Select(up => up.Date.Date) // Luăm doar datele unice
                .Distinct()
                .ToList();
            userObjectiveProgresses = userObjectiveProgresses.OrderByDescending(up => up.Date).ToList();
            int daysStreak = 0;
            DateTime today = DateTime.UtcNow.Date;
            foreach (var x in userObjectiveProgresses)
            {
                Console.WriteLine("userObjectiveProgresses: " + x);
            }
                
            Console.WriteLine("nr: " + userObjectiveProgresses.Count);

            // Verificăm dacă utilizatorul a completat obiective pentru ziua de azi
            if (!userObjectiveProgresses.Contains(today))
                today = today.AddDays(-1); // Dacă azi nu a completat, începem streak-ul de ieri
            

            // Parcurgem zilele consecutive
            foreach (var date in userObjectiveProgresses)
            {
                Console.WriteLine("date: " + date);
                Console.WriteLine("today: " + today);
                if (date == today)
                {
                    daysStreak++;
                    today = today.AddDays(-1); // Verificăm ziua anterioară
                }
                else
                {
                    break; // Dacă o zi lipsește, streak-ul s-a întrerupt
                }
            }

            return daysStreak;
        }

        // formularul in care se vor completa datele unei profil nouu
        [Authorize(Roles = "Admin,Moderator,User")]
        public IActionResult New()
        {
            ApplicationUser user = new ApplicationUser();

            return View(user);
        }


        // Se adauga profilul in baza de date
        [Authorize(Roles = "Admin,Moderator,User")]

        [HttpPost]
        public IActionResult New(ApplicationUser updatedUser)
        {

            // Obține ID-ul utilizatorului autentificat
            string userId = _userManager.GetUserId(User);

            // Găsește utilizatorul în baza de date
            ApplicationUser user = db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                // Dacă utilizatorul nu este găsit, întoarce o eroare
                TempData["message"] = "User not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Actualizează câmpurile utilizatorului cu datele din formular
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Description = updatedUser.Description;
            user.Birthday = updatedUser.Birthday;

            // Salvează modificările în baza de date
            db.SaveChanges();

            TempData["message"] = "Your profile has been updated successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Show", new { id = userId }); // Redirecționează către profilul utilizatorului


        }


        // HttpGet implicit
        // Se afiseaza formularul impreuna cu datele aferente profilului din baza de date

        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ApplicationUser user = db.ApplicationUsers
                                        .Where(u => u.Id == _userManager.GetUserId(User))
                                        .First();

            user.Id = _userManager.GetUserId(User);
            ViewBag.User = user;
            return View(user);
        }

        // Se adauga profilul modificat in baza de date
        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpPost]
        public IActionResult Edit(string id, ApplicationUser requestProfile)
        {
            ApplicationUser user = db.ApplicationUsers.Find(id);
            if(id!= _userManager.GetUserId(User) && User.IsInRole("Admin") == false)
            {
                TempData["message"] = "Access denied.";
                TempData["messageType"] = "alert-danger";
                // Redirecționează înapoi la pagina anterioară
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                {
                    return Redirect(referer);
                }
            }
            user.FirstName = requestProfile.FirstName;
            user.LastName = requestProfile.LastName;
            user.Description = requestProfile.Description;
            user.Birthday = requestProfile.Birthday;

            TempData["message"] = "You edited your profile successfully!";
            TempData["messageType"] = "alert-success";
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin,Moderator,User")]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUser user = db.ApplicationUsers
                                   .Where(user => user.Id == id).First();



            if (user == null)
            {
                // Handle the case where user is not found
                return RedirectToAction("Index");
            }

            if (user.Id == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Moderator"))
            {
                // Delete associated diary entries
                var diariesToDelete = db.Diaries.Where(d => d.UserId == user.Id).ToList();
                db.Diaries.RemoveRange(diariesToDelete);
                db.ApplicationUsers.Remove(user);
                TempData["message"] = "Your profile has been deleted.";
                TempData["messageType"] = "alert-success";
                db.SaveChanges();

                await _signInManager.SignOutAsync();

                // Redirect to the Index page
                return RedirectToAction("Index");

            }

            else return RedirectToAction("Index");

        }

        // Formularul pentru crearea unui profil de terapeut
        [Authorize(Roles = "Admin,Moderator,User")]
        public IActionResult NewTherapist()
        {
            ApplicationUser therapist = new ApplicationUser();
            return View(therapist);
        }

        // Se adaugă un profil de terapeut în baza de date
        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpPost]
        public IActionResult NewTherapist(ApplicationUser updatedTherapist)
        {
            string userId = _userManager.GetUserId(User);

            // Găsește utilizatorul autentificat
            ApplicationUser user = db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                TempData["message"] = "User not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Setează valorile pentru terapeut
            user.FirstName = updatedTherapist.FirstName;
            user.LastName = updatedTherapist.LastName;
            user.Description = updatedTherapist.Description;
            user.Birthday = updatedTherapist.Birthday;
            user.IsTherapist = true;
            user.ProfilePicture = updatedTherapist.ProfilePicture;
            user.Studies = updatedTherapist.Studies;
            user.PhoneNumber = updatedTherapist.PhoneNumber;
            user.BackgroundColor = updatedTherapist.BackgroundColor;
            user.Rating = 5;
            user.NumberOfReviews = 0;

            // Salvează modificările în baza de date
            db.SaveChanges();

            TempData["message"] = "Your therapist profile has been updated successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Show", new { id = userId });
        }

        // Formularul pentru editarea unui profil de terapeut existent
        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpGet]
        public IActionResult EditTherapist(string id)
        {
            SetAccessRights();

            ApplicationUser therapist = db.ApplicationUsers
                                          .Where(u => u.Id == id)
                                          .FirstOrDefault();

            if (therapist == null)
            {
                TempData["message"] = "Therapist not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            ViewBag.User = therapist;
            return View(therapist);
        }

        // Se salvează modificările aduse profilului de terapeut
        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpPost]
        public IActionResult EditTherapist(string id, ApplicationUser updatedTherapist)
        {
            SetAccessRights();

            ApplicationUser therapist = db.ApplicationUsers.Find(id);

            if (therapist == null)
            {
                TempData["message"] = "Therapist not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Actualizează profilul de terapeut
            therapist.ProfilePicture = updatedTherapist.ProfilePicture;
            therapist.Studies = updatedTherapist.Studies;
            therapist.PhoneNumber = updatedTherapist.PhoneNumber;
            therapist.Birthday = updatedTherapist.Birthday;
            therapist.BackgroundColor = updatedTherapist.BackgroundColor;
            therapist.Rating = updatedTherapist.Rating;
            therapist.NumberOfReviews = updatedTherapist.NumberOfReviews;

            db.SaveChanges();

            TempData["message"] = "Your therapist profile has been updated successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Show", new { id = therapist.Id });
        }


    }

}