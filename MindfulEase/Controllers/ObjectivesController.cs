using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindfulEase.Data;
using MindfulEase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace MindfulEase.Controllers
{
    public class ObjectivesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public ObjectivesController(ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteModerator = User.IsInRole("Moderator");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
        [Authorize(Roles = "Admin,Moderator,User")]
        public IActionResult Index()
        {
            SetAccessRights();
            var objectives = db.Objectives.ToList();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userObjectives = db.UserObjectives
                .Where(uo => uo.UserId == userId)
                .ToList();

            ViewBag.UserObjectives = userObjectives;
            ViewBag.UserId = userId;

            return View(objectives);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Moderator,User")]
        public IActionResult SetUserObjective(int ObjectiveId, int? TargetValue, int? TargetTime)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userObjective = db.UserObjectives.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectiveId == ObjectiveId);

            if (userObjective != null)
            {
                // Edităm obiectivul existent
                userObjective.TargetValue = TargetValue;
                userObjective.TargetTime = TargetTime.HasValue ? TimeSpan.FromMinutes(TargetTime.Value) : null;
            }
            else
            {
                // Creăm un obiectiv nou pentru utilizator
                userObjective = new UserObjective
                {
                    UserId = userId,
                    ObjectiveId = ObjectiveId,
                    TargetValue = TargetValue,
                    TargetTime = TargetTime.HasValue ? TimeSpan.FromMinutes(TargetTime.Value) : null
                };
                db.UserObjectives.Add(userObjective);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult New(Objective objective)
        {
            if (ModelState.IsValid)
            {
                db.Objectives.Add(objective);
                db.SaveChanges();
                TempData["message"] = "Objective created successfully!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            TempData["message"] = "There was an error creating the objective.";
            TempData["messageType"] = "alert-danger";
            return View(objective);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Edit(int id)
        {
            var objective = db.Objectives.FirstOrDefault(q => q.Id == id);
            if (objective == null)
            {
                TempData["message"] = "Objective not found.";
                TempData["messageType"] = "alert-danger";
                return NotFound();
            }
            return View(objective);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, Objective updatedObjective)
        {
            var objective = db.Objectives.FirstOrDefault(o => o.Id == id);
            if (objective == null)
            {
                TempData["message"] = "Objective not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            objective.Title = updatedObjective.Title;
            objective.ValueType = updatedObjective.ValueType;
            objective.Category = updatedObjective.Category;
            db.SaveChanges();
            TempData["message"] = "Objective updated successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var objective = db.Objectives.Find(id);
            if (User.IsInRole("Admin") == false && User.IsInRole("Moderator") == false)
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
            if (objective != null)
            {
                // Găsim toate înregistrările UserObjective asociate
                var userObjectives = db.UserObjectives.Where(uo => uo.ObjectiveId == id).ToList();

                foreach (var userObjective in userObjectives)
                {
                    // Ștergem progresul asociat fiecărui UserObjective
                    var progressEntries = db.UserObjectiveProgresses.Where(p => p.UserObjectiveId == userObjective.Id).ToList();
                    db.UserObjectiveProgresses.RemoveRange(progressEntries);

                    // Ștergem UserObjective
                    db.UserObjectives.Remove(userObjective);
                }

                // Ștergem obiectivul principal
                db.Objectives.Remove(objective);

                db.Objectives.Remove(objective);
                db.SaveChanges();
            }
            TempData["message"] = "Objective not found.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CompleteObjective(int objectiveId)
        {
            var userObjectiveProgress = db.UserObjectiveProgresses
                .FirstOrDefault(p => p.UserObjectiveId == objectiveId && p.Date.Date == DateTime.Now.Date);
            Console.WriteLine("ObjectiveId: " + objectiveId.ToString());
            if (userObjectiveProgress == null)
            {
                // Dacă nu există progres, îl creăm
                userObjectiveProgress = new UserObjectiveProgress
                {
                    UserObjectiveId = objectiveId,
                    Date = DateTime.Now.Date,
                    IsCompleted = true
                };
                db.UserObjectiveProgresses.Add(userObjectiveProgress);
            }
            else
            {
                // Dacă există, actualizăm statusul la completat
                userObjectiveProgress.IsCompleted = true;
            }

            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult UndoCompleteObjective(int objectiveId)
        {
            var userObjectiveProgress = db.UserObjectiveProgresses
                .FirstOrDefault(p => p.UserObjectiveId == objectiveId && p.Date.Date == DateTime.Now.Date);

            if (userObjectiveProgress != null)
            {
                // Dacă există progres, îl setăm ca incomplet
                userObjectiveProgress.IsCompleted = false;
                db.SaveChanges();
            }

            return Json(new { success = true });
        }

    }
}
