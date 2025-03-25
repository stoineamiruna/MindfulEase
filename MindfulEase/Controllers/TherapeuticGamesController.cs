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
            ViewBag.Tags = db.Tags.ToList();
            return View();
        }

        // Adaugă jocul terapeutic în baza de date
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New(TherapeuticGame game, List<int> SelectedTags)
        {
            if (ModelState.IsValid)
            {
                db.TherapeuticGames.Add(game);
                db.SaveChanges();

                // Asociază tagurile selectate
                if (SelectedTags != null && SelectedTags.Any())
                {
                    foreach (var tagId in SelectedTags)
                    {
                        db.TherapeuticGameTags.Add(new TherapeuticGameTag { TherapeuticGameId = game.Id, TagId = tagId });
                    }
                    db.SaveChanges();
                }

                TempData["message"] = "Therapeutic game created successfully!";
                TempData["messageType"] = "alert-success";


                return RedirectToAction("Play", new { id = game.Id });
            }

            TempData["message"] = "There was an error creating the therapeutic game.";
            TempData["messageType"] = "alert-danger";
            ViewBag.Tags = db.Tags.ToList();
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

            ViewBag.Tags = db.Tags.ToList(); // Toate tagurile disponibile

            var selectedTags = db.TherapeuticGameTags
                      .Where(rt => rt.TherapeuticGameId == id)
                      .Select(rt => rt.TagId) // Selectează doar ID-urile tagurilor
                      .Where(tagId => tagId.HasValue) // Filtrează doar tagurile care nu sunt null
                      .Select(tagId => tagId.Value) // Extrage valoarea de tip `int`
                      .ToList();

            ViewBag.SelectedTags = selectedTags;

            return View(game);
        }

        // POST: Actualizează detaliile unui joc terapeutic
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, TherapeuticGame updatedGame, List<int> SelectedTags)
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

            if (game.Tags == null)
            {
                game.Tags = new List<TherapeuticGameTag>();
            }

            var existingTags = db.TherapeuticGameTags.Where(rt => rt.TherapeuticGameId == game.Id).ToList();
            db.TherapeuticGameTags.RemoveRange(existingTags);

            // Adaugă tagurile noi
            if (SelectedTags != null && SelectedTags.Any())
            {
                foreach (var tagId in SelectedTags)
                {
                    db.TherapeuticGameTags.Add(new TherapeuticGameTag { TherapeuticGameId = game.Id, TagId = tagId });
                }
            }

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

            var therapeuticGameTags = db.TherapeuticGameTags.Where(sr => sr.TherapeuticGameId == id).ToList();
            db.TherapeuticGameTags.RemoveRange(therapeuticGameTags);

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
                // Căutare în tabela TherapeuticGames după nume
                allGames = db.TherapeuticGames
                    .Where(g => g.Name.Contains(search) ||
                                db.TherapeuticGameTags.Any(tgt => tgt.TherapeuticGameId == g.Id && tgt.Tag.Title.Contains(search)))
                    .ToList();

                // Căutare în tabela Quizzes după titlu, descriere sau taguri asociate
                allQuizzes = db.Quizzes
                    .Where(q => q.Title.Contains(search) || q.Description.Contains(search) ||
                                db.QuizTags.Any(qt => qt.QuizId == q.Id && qt.Tag.Title.Contains(search)))
                    .ToList();

                // Căutare în jocurile utilizatorului după nume sau taguri
                gamesUser = db.ApplicationUserTherapeuticGames
                    .Where(autg => autg.UserId == userId &&
                                   (autg.Game.Name.Contains(search) ||
                                    db.TherapeuticGameTags.Any(tgt => tgt.TherapeuticGameId == autg.Game.Id && tgt.Tag.Title.Contains(search))))
                    .Select(autg => autg.Game)
                    .ToList();

                // Căutare în quizurile utilizatorului după titlu, descriere sau taguri asociate
                quizzesUser = db.ApplicationUserQuizzes
                    .Where(auq => auq.UserId == userId &&
                                  (auq.Quiz.Title.Contains(search) || auq.Quiz.Description.Contains(search) ||
                                   db.QuizTags.Any(qt => qt.QuizId == auq.Quiz.Id && qt.Tag.Title.Contains(search))))
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
                games = games.Where(g => g.Name.ToLower().Contains(search.ToLower()) ||
                         db.TherapeuticGameTags
                           .Join(db.Tags, tgt => tgt.TagId, t => t.Id, (tgt, t) => new { tgt.TherapeuticGameId, t.Title })
                           .Any(joined => joined.TherapeuticGameId == g.Id &&
                                          joined.Title.ToLower().Contains(search.ToLower())))
                            .ToList();

                quizzes = quizzes.Where(q => q.Title.ToLower().Contains(search.ToLower()) ||
                                             q.Description.ToLower().Contains(search.ToLower()) ||
                                             db.QuizTags
                                               .Join(db.Tags, qt => qt.TagId, t => t.Id, (qt, t) => new { qt.QuizId, t.Title })
                                               .Any(joined => joined.QuizId == q.Id &&
                                                              joined.Title.ToLower().Contains(search.ToLower())))
                                .ToList();



            }

            ViewBag.Type = type;
            ViewBag.SearchString = search;
            ViewBag.Games = games;
            ViewBag.Quizzes = quizzes;

            return View();
        }


    }


}

