using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindfulEase.Data;
using MindfulEase.Models;

namespace MindfulEase.Controllers
{
    public class TherapeuticGamesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public TherapeuticGamesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        private void SetAccessRights()
        {
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.EsteModerator = User.IsInRole("Moderator");
        }

        // Afișează formularul pentru crearea unui joc terapeutic nou
        [Authorize(Roles = "Admin, Moderator")]
        public IActionResult New()
        {
            return View();
        }

        // Adaugă jocul terapeutic în baza de date
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New(TherapeuticGame game)
        {
            if (ModelState.IsValid)
            {
                db.TherapeuticGames.Add(game);
                db.SaveChanges();

                TempData["message"] = "Therapeutic game created successfully!";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Play", new { id = game.Id });
            }

            TempData["message"] = "There was an error creating the therapeutic game.";
            TempData["messageType"] = "alert-danger";
            return View(game);
        }

        // GET: Editarea unui joc terapeutic existent
        [Authorize(Roles = "Admin, Moderator")]
        public IActionResult Edit(int id)
        {
            var game = db.TherapeuticGames.FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                TempData["message"] = "Therapeutic game not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            return View(game);
        }

        // POST: Actualizează detaliile unui joc terapeutic
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, TherapeuticGame updatedGame)
        {
            var game = db.TherapeuticGames.FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                TempData["message"] = "Therapeutic game not found.";
                TempData["messageType"] = "alert-danger";
                return View("Play", game.Id);
            }

            game.Name = updatedGame.Name;
            game.Type = updatedGame.Type;
            game.Instructions = updatedGame.Instructions;
            game.GameUrl = updatedGame.GameUrl;
            game.Background = updatedGame.Background;

            db.SaveChanges();

            TempData["message"] = "Therapeutic game updated successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Play", new { id = game.Id });
        }

        // POST: Șterge un joc terapeutic
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var game = db.TherapeuticGames.FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                TempData["message"] = "Therapeutic game not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            var applicationUserTherapeuticGames = db.ApplicationUserTherapeuticGames.Where(sr => sr.GameId == id).ToList();
            db.ApplicationUserTherapeuticGames.RemoveRange(applicationUserTherapeuticGames);
            db.TherapeuticGames.Remove(game);
            db.SaveChanges();

            TempData["message"] = "Therapeutic game deleted successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }

        public IActionResult Play(int id)
        {
            SetAccessRights();
            var game = db.TherapeuticGames.FirstOrDefault(g => g.Id == id);
            if (game == null) return NotFound();

            return View(game);
        }

        public IActionResult Index(string search)
        {
            SetAccessRights();
            var userId = _userManager.GetUserId(User);

            var allGames = db.TherapeuticGames.ToList();
            var allQuizzes = db.Quizzes.ToList();


            var gamesUser = db.ApplicationUserTherapeuticGames
                .Where(autg => autg.UserId == userId)
                .Select(autg => autg.Game)
                .ToList();

            var quizzesUser = db.ApplicationUserQuizzes
                .Where(auq => auq.UserId == userId)
                .Select(auq => auq.Quiz)
                .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                //users = users.Where(a => a.FirstName.Contains(search) || a.LastName.Contains(search));
                allGames = db.TherapeuticGames.Where(g => g.Name.Contains(search)).ToList();
                allQuizzes = db.Quizzes.Where(q => q.Title.Contains(search) || q.Description.Contains(search)).ToList();
                gamesUser = db.ApplicationUserTherapeuticGames
                .Where(autg => (autg.UserId == userId) && autg.Game.Name.Contains(search))
                .Select(autg => autg.Game)
                .ToList();

                quizzesUser = db.ApplicationUserQuizzes
                    .Where(auq => (auq.UserId == userId) && (auq.Quiz.Title.Contains(search) || auq.Quiz.Description.Contains(search)))
                    .Select(auq => auq.Quiz)
                    .ToList();
            }
            ViewBag.AllGames = allGames;
            ViewBag.AllQuizzes = allQuizzes;
            ViewBag.QuizzesUser = quizzesUser;
            ViewBag.GamesUser = gamesUser;
            ViewBag.SearchString = search;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }

        public IActionResult SeeMore(string type, string search = "")
        {
            SetAccessRights();
            var userId = _userManager.GetUserId(User);

            List<TherapeuticGame> games = new List<TherapeuticGame>();
            List<Quiz> quizzes = new List<Quiz>();

            Console.WriteLine(type);
            Console.WriteLine(search);

            switch (type)
            {
                case "myQuizzes":
                    quizzes = db.ApplicationUserQuizzes
                        .Where(auq => auq.UserId == userId)
                        .Select(auq => auq.Quiz)
                        .ToList();
                    break;
                case "allGames":
                    games = db.TherapeuticGames.ToList();
                    break;
                case "allQuizzes":
                    quizzes = db.Quizzes.ToList();
                    break;
            }

            if (!string.IsNullOrEmpty(search))
            {
                games = games.Where(g => g.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                quizzes = quizzes.Where(q => q.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                             q.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.Type = type;
            ViewBag.SearchString = search;
            ViewBag.Games = games;
            ViewBag.Quizzes = quizzes;

            return View();
        }


    }


}

