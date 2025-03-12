using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Services;

namespace MindfulEase.Controllers
{
    public class WeeklyChallengesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RecommendationService _recommendationService;


        public WeeklyChallengesController(ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager,
           RecommendationService recommendationService)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _recommendationService = recommendationService;
        }

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteModerator = User.IsInRole("Moderator");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var currentDate = DateTime.UtcNow;

            var myChallenges = db.ApplicationUserWeeklyChallenges
                .Where(auc => auc.UserId == userId && auc.IsCompleted)
                .Select(auc => auc.WeeklyChallenge)
                .OrderByDescending(wc => wc.StartDate)
                .ToList();

            var activeChallenges = db.WeeklyChallenges
                .Where(wc => wc.StartDate <= currentDate && wc.EndDate >= currentDate)
                .OrderByDescending(wc => wc.StartDate)
                .ToList();

            var comingSoon = db.WeeklyChallenges
                .Where(wc => wc.StartDate > currentDate)
                .OrderBy(wc => wc.StartDate)
                .ToList();

            var pastChallenges = db.WeeklyChallenges
                .Where(wc => wc.EndDate < currentDate)
                .OrderByDescending(wc => wc.StartDate)
                .ToList();

            var myChallengesWithRequirements = new List<dynamic>();
            foreach (var challenge in myChallenges)
            {
                var userPoints = await GetUserPointsCountAsync(challenge.Id, userId);
                var userJournalEntries = await GetUserJournalEntriesCountAsync(challenge.Id, userId);
                var userQuizzes = await GetUserQuizzesCountAsync(challenge.Id, userId);
                var isCompleted = await IsChallengeCompletedAsync(challenge.Id, userId);
                myChallengesWithRequirements.Add(new
                {
                    Id = challenge.Id,
                    Title = challenge.Title,
                    Description = challenge.Description,
                    StartDate = challenge.StartDate,
                    EndDate = challenge.EndDate,
                    RequiredPoints = challenge.RequiredPoints,
                    RequiredJournalEntries = challenge.RequiredJournalEntries,
                    RequiredQuizzes = challenge.RequiredQuizzes,
                    RequiredResources = challenge.RequiredResources,
                    URLBackground = challenge.URLBackground,
                    IsCompleted = isCompleted,
                    UserPoints = userPoints,
                    UserJournalEntries = userJournalEntries,
                    UserQuizzes = userQuizzes,
                });
            }

            var activeChallengesWithRequirements = new List<dynamic>();
            foreach (var challenge in activeChallenges)
            {
                var userPoints = await GetUserPointsCountAsync(challenge.Id, userId);
                var userJournalEntries = await GetUserJournalEntriesCountAsync(challenge.Id, userId);
                var userQuizzes = await GetUserQuizzesCountAsync(challenge.Id, userId);
                var isCompleted = await IsChallengeCompletedAsync(challenge.Id, userId);
                activeChallengesWithRequirements.Add(new
                {
                    Id = challenge.Id,
                    Title = challenge.Title,
                    Description = challenge.Description,
                    StartDate = challenge.StartDate,
                    EndDate = challenge.EndDate,
                    RequiredPoints = challenge.RequiredPoints,
                    RequiredJournalEntries = challenge.RequiredJournalEntries,
                    RequiredQuizzes = challenge.RequiredQuizzes,
                    RequiredResources = challenge.RequiredResources,
                    URLBackground = challenge.URLBackground,
                    IsCompleted = isCompleted,
                    UserPoints = userPoints,
                    UserJournalEntries = userJournalEntries,
                    UserQuizzes = userQuizzes,
                });
            }

            var pastChallengesWithRequirements = new List<dynamic>();
            foreach (var challenge in pastChallenges)
            {
                var userPoints = await GetUserPointsCountAsync(challenge.Id, userId);
                var userJournalEntries = await GetUserJournalEntriesCountAsync(challenge.Id, userId);
                var userQuizzes = await GetUserQuizzesCountAsync(challenge.Id, userId);
                var isCompleted = await IsChallengeCompletedAsync(challenge.Id, userId);
                pastChallengesWithRequirements.Add(new
                {
                    Id = challenge.Id,
                    Title = challenge.Title,
                    Description = challenge.Description,
                    StartDate = challenge.StartDate,
                    EndDate = challenge.EndDate,
                    RequiredPoints = challenge.RequiredPoints,
                    RequiredJournalEntries = challenge.RequiredJournalEntries,
                    RequiredQuizzes = challenge.RequiredQuizzes,
                    RequiredResources = challenge.RequiredResources,
                    URLBackground = challenge.URLBackground,
                    IsCompleted = isCompleted,
                    UserPoints = userPoints,
                    UserJournalEntries = userJournalEntries,
                    UserQuizzes = userQuizzes,
                });
            }

            var comingSoonWithRequirements = new List<dynamic>();
            foreach (var challenge in comingSoon)
            {
                var userPoints = await GetUserPointsCountAsync(challenge.Id, userId);
                var userJournalEntries = await GetUserJournalEntriesCountAsync(challenge.Id, userId);
                var userQuizzes = await GetUserQuizzesCountAsync(challenge.Id, userId);
                var isCompleted = await IsChallengeCompletedAsync(challenge.Id, userId);
                comingSoonWithRequirements.Add(new
                {
                    Id = challenge.Id,
                    Title = challenge.Title,
                    Description = challenge.Description,
                    StartDate = challenge.StartDate,
                    EndDate = challenge.EndDate,
                    RequiredPoints = challenge.RequiredPoints,
                    RequiredJournalEntries = challenge.RequiredJournalEntries,
                    RequiredQuizzes = challenge.RequiredQuizzes,
                    RequiredResources = challenge.RequiredResources,
                    URLBackground = challenge.URLBackground,
                    IsCompleted = isCompleted,
                    UserPoints = userPoints,
                    UserJournalEntries = userJournalEntries,
                    UserQuizzes = userQuizzes,
                });
            }

            ViewBag.MyChallenges = myChallengesWithRequirements;
            ViewBag.ActiveChallenges = activeChallengesWithRequirements;
            ViewBag.ComingSoon = comingSoonWithRequirements;
            ViewBag.PastChallenges = pastChallengesWithRequirements;


            var user = db.ApplicationUsers.Find(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Dacă utilizatorul nu are ClusterId, îi asignăm unul
            if (user.ClusterId == null)
            {
                var clusteringService = new ClusteringService(db);
                clusteringService.AssignClustersToUsers();
                user = db.ApplicationUsers.Find(userId); // Reîncarcă utilizatorul cu ClusterId actualizat
            }

            var recommendedResources = _recommendationService.GetRecommendedResources(userId);
            var allResources = db.Resources.ToList();

            // Creăm o listă combinată cu resursele recomandate urmate de celelalte resurse
            var combinedResources = new List<Resource>();

            // Adăugăm resursele recomandate
            combinedResources.AddRange(recommendedResources);

            // Adăugăm restul resurselor care nu sunt deja în lista de recomandări
            combinedResources.AddRange(allResources.Where(r => !recommendedResources.Contains(r)));

            ViewBag.Resources = combinedResources.Take(4).ToList();
            SetAccessRights(); 
            return View();
        }

        public async Task<bool> IsChallengeCompletedAsync(int challengeId, string userId)
        {
            var challengeEntry = await db.ApplicationUserWeeklyChallenges
                .FirstOrDefaultAsync(x => x.UserId == userId && x.WeeklyChallengeId == challengeId);

            // Dacă găsește intrarea și este completată, returnează true, altfel false
            return challengeEntry != null && challengeEntry.IsCompleted;
        }

        public async Task<int> GetUserPointsCountAsync(int challengeId, string userId)
        {
            var challenge = await db.WeeklyChallenges.FindAsync(challengeId);
            if (challenge == null || !challenge.RequiredPoints.HasValue)
            {
                return 0; // Dacă nu există cerință pentru puncte, returnăm 0
            }

            var userPoints = await db.Rewards
                .Where(a => a.UserId == userId && a.DateEarned >= challenge.StartDate && a.DateEarned <= challenge.EndDate)
                .SumAsync(a => a.Points);

            return userPoints; // Returnăm numărul total de puncte câștigate
        }


        public async Task<int> GetUserJournalEntriesCountAsync(int challengeId, string userId)
        {
            var challenge = await db.WeeklyChallenges.FindAsync(challengeId);
            if (challenge == null || !challenge.RequiredJournalEntries.HasValue)
            {
                return 0; // Dacă nu există cerință pentru jurnal, returnăm 0
            }

            var journalEntries = await db.Diaries
                .Where(d => d.UserId == userId && d.EntryDate >= challenge.StartDate && d.EntryDate <= challenge.EndDate)
                .CountAsync();

            return journalEntries; // Returnăm numărul de intrări în jurnal
        }


        public async Task<int> GetUserQuizzesCountAsync(int challengeId, string userId)
        {
            var challenge = await db.WeeklyChallenges.FindAsync(challengeId);
            if (challenge == null || !challenge.RequiredQuizzes.HasValue)
            {
                return 0; // Dacă nu există cerință pentru quizuri, returnăm 0
            }

            var quizzesCompleted = await db.ApplicationUserQuizzes
                .Where(q => q.UserId == userId && q.Date >= challenge.StartDate && q.Date <= challenge.EndDate)
                .CountAsync();

            return quizzesCompleted; // Returnăm numărul de quizuri completate
        }





        public async Task<IActionResult> SeeMore(string category)
        {
            var userId = _userManager.GetUserId(User);
            var currentDate = DateTime.UtcNow;

            // Lista provocărilor pentru fiecare categorie
            List<WeeklyChallenge> challenges = category switch
            {
                "MyChallenges" => db.ApplicationUserWeeklyChallenges
                    .Where(auc => auc.UserId == userId && auc.IsCompleted)
                    .Select(auc => auc.WeeklyChallenge)
                    .OrderByDescending(wc => wc.StartDate)
                    .ToList(),

                "ActiveChallenges" => db.WeeklyChallenges
                    .Where(wc => wc.StartDate <= currentDate && wc.EndDate >= currentDate)
                    .OrderByDescending(wc => wc.StartDate)
                    .ToList(),

                "ComingSoon" => db.WeeklyChallenges
                    .Where(wc => wc.StartDate > currentDate)
                    .OrderBy(wc => wc.StartDate)
                    .ToList(),

                "PastChallenges" => db.WeeklyChallenges
                    .Where(wc => wc.EndDate < currentDate)
                    .OrderByDescending(wc => wc.StartDate)
                    .ToList(),

                _ => new List<WeeklyChallenge>()
            };

            // Lista finală de provocări cu informațiile utilizatorului
            var challengesWithDetails = new List<dynamic>();
            foreach (var challenge in challenges)
            {
                // Obține punctele, jurnalul și quizurile pentru fiecare provocare
                var userPoints = await GetUserPointsCountAsync(challenge.Id, userId);
                var userJournalEntries = await GetUserJournalEntriesCountAsync(challenge.Id, userId);
                var userQuizzes = await GetUserQuizzesCountAsync(challenge.Id, userId);
                var isCompleted = await IsChallengeCompletedAsync(challenge.Id, userId);

                challengesWithDetails.Add(new
                {
                    Id = challenge.Id,
                    Title = challenge.Title,
                    Description = challenge.Description,
                    StartDate = challenge.StartDate,
                    EndDate = challenge.EndDate,
                    RequiredPoints = challenge.RequiredPoints,
                    RequiredJournalEntries = challenge.RequiredJournalEntries,
                    RequiredQuizzes = challenge.RequiredQuizzes,
                    RequiredResources = challenge.RequiredResources,
                    URLBackground = challenge.URLBackground,
                    IsCompleted = isCompleted,
                    UserPoints = userPoints,
                    UserJournalEntries = userJournalEntries,
                    UserQuizzes = userQuizzes,
                });
            }

            // Afișarea numelui categoriei
            ViewBag.CategoryName = category.Replace("Challenges", " Challenges");
            ViewBag.Challenges = challengesWithDetails;

            // Adăugarea resurselor recomandate
            var user = db.ApplicationUsers.Find(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Dacă utilizatorul nu are ClusterId, îi asignăm unul
            if (user.ClusterId == null)
            {
                var clusteringService = new ClusteringService(db);
                clusteringService.AssignClustersToUsers();
                user = db.ApplicationUsers.Find(userId); // Reîncarcă utilizatorul cu ClusterId actualizat
            }

            var recommendedResources = _recommendationService.GetRecommendedResources(userId);
            var allResources = db.Resources.ToList();

            // Creăm o listă combinată cu resursele recomandate urmate de celelalte resurse
            var combinedResources = new List<Resource>();

            // Adăugăm resursele recomandate
            combinedResources.AddRange(recommendedResources);

            // Adăugăm restul resurselor care nu sunt deja în lista de recomandări
            combinedResources.AddRange(allResources.Where(r => !recommendedResources.Contains(r)));

            // Afișarea resurselor în ViewBag
            ViewBag.Resources = combinedResources.Take(4).ToList();

            // Setarea drepturilor de acces pentru butoane
            SetAccessRights();

            // Returnează vizualizarea "SeeMore"
            return View("SeeMore");
        }



        [HttpPost]
        public async Task<IActionResult> Edit(WeeklyChallenge challenge)
        {
            if (ModelState.IsValid)
            {
                db.WeeklyChallenges.Update(challenge);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(challenge);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var challenge = await db.WeeklyChallenges.FindAsync(id);
            if (challenge != null)
            {
                db.WeeklyChallenges.Remove(challenge);
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WeeklyChallenge challenge)
        {
            if (ModelState.IsValid)
            {
                db.WeeklyChallenges.Add(challenge);
                await db.SaveChangesAsync();

               
                return RedirectToAction("Index");
            }

            return View(challenge);
        }




    }
}
