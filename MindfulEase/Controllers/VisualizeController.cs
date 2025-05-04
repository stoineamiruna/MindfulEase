using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Models.ML;
using MindfulEase.Services;
using Newtonsoft.Json;

namespace MindfulEase.Controllers
{
    public class VisualizeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly BrainDamagePredictionService _predictionService;
        public VisualizeController(ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager, 
           SignInManager<ApplicationUser> signInManager,
           BrainDamagePredictionService predictionService)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _predictionService = predictionService;
        }
        public async Task<IActionResult> Brain()
        {
            var userId = _userManager.GetUserId(User);

            // Preluăm emoțiile pentru toate datele disponibile
            var userEmotionsData = await db.ApplicationUserEmotions
                .Where(ue => ue.UserId == userId)
                .Select(ue => new
                {
                    Date = ue.Date.Date,
                    EmotionLabel = ue.Emotion.Label.ToLower(),
                    MoodValue = (double)ue.MoodValue
                })
                .ToListAsync();

            var diaryEmotionsData = await db.DiaryEmotions
                .Where(de => de.Diary.UserId == userId)
                .Select(de => new
                {
                    Date = de.Diary.EntryDate.Date,
                    EmotionLabel = de.Emotion.Label.ToLower(),
                    MoodValue = (double)(de.Score * 9 + 1) // Conversie de la 0-1 la 1-10
                })
                .ToListAsync();

            var allEmotions = userEmotionsData.Concat(diaryEmotionsData)
                .GroupBy(e => new { e.Date, e.EmotionLabel })
                .Select(g => new
                {
                    g.Key.Date,
                    g.Key.EmotionLabel,
                    AverageMoodValue = g.Average(e => e.MoodValue) // Calculăm media intensității emoțiilor pe zi
                })
                .ToList();

            ViewBag.EmotionsByDate = JsonConvert.SerializeObject(allEmotions); // Trimitem toate emoțiile
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> PredictBrainDamage([FromBody] PredictionRequest request)
        {
            var userId = _userManager.GetUserId(User);
            Console.WriteLine($"[PredictBrainDamage] UserId: {userId}");
            Console.WriteLine($"[PredictBrainDamage] Request Date: {request.Date}, YearsSinceStart: {request.YearsSinceStart}");

            var allEmotions = await db.ApplicationUserEmotions
                .Where(ue => ue.UserId == userId && ue.Date.Date == request.Date.Date)
                .Select(ue => new { ue.Emotion.Label, MoodValue = (int)Math.Round((double)ue.MoodValue) })
                .ToListAsync();

            Console.WriteLine($"[PredictBrainDamage] Found {allEmotions.Count} ApplicationUserEmotions");

            var diaryEmotions = await db.DiaryEmotions
                .Where(de => de.Diary.UserId == userId && de.Diary.EntryDate.Date == request.Date.Date)
                .Select(de => new { de.Emotion.Label, MoodValue = (int)Math.Round((double)(de.Score * 9 + 1)) })
                .ToListAsync();

            Console.WriteLine($"[PredictBrainDamage] Found {diaryEmotions.Count} DiaryEmotions");

            var grouped = allEmotions.Concat(diaryEmotions)
                .GroupBy(e => e.Label.ToLower())
                .ToDictionary(g => g.Key, g => (int)Math.Round(g.Average(e => e.MoodValue)));

            Console.WriteLine("[PredictBrainDamage] Grouped emotions:");
            foreach (var kvp in grouped)
            {
                Console.WriteLine($"  - {kvp.Key}: {kvp.Value}");
            }

            int Get(string key) => grouped.TryGetValue(key, out var val) ? val : 1;

            var user = await _userManager.GetUserAsync(User);
            Console.WriteLine($"[PredictBrainDamage] User Age: {user.Age}, Sex: {user.Sex}");

            var input = new EmotionalInputData
            {
                Age = (float)user.Age,
                Sex = user.Sex,
                Joy = Get("joy"),
                Sadness = Get("sadness"),
                Anger = Get("anger"),
                Love = Get("love"),
                Fear = Get("fear"),
                Surprise = Get("surprise"),
                Disgust = Get("disgust"),
                YearsSinceStart = request.YearsSinceStart
            };

            Console.WriteLine("[PredictBrainDamage] Input for model:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(input));

            var prediction = _predictionService.PredictBrainDamageProgression(input);

            Console.WriteLine("[PredictBrainDamage] Prediction result:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(prediction));

            return Json(prediction);
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> PredictBrainDamageWeighted([FromBody] PredictionRequest request)
        {
            var userId = _userManager.GetUserId(User);
            Console.WriteLine($"[PredictBrainDamage] UserId: {userId}");
            Console.WriteLine($"[PredictBrainDamage] Request YearsSinceStart: {request.YearsSinceStart}");

            // Calculăm data de început pe baza valorii sliderului (YearsSinceStart)
            var startDate = DateTime.Now.AddYears(-request.YearsSinceStart);

            // Extragem emoțiile utilizatorului din baza de date
            var allEmotions = await db.ApplicationUserEmotions
                .Where(ue => ue.UserId == userId && ue.Date >= startDate)
                .Select(ue => new { ue.Emotion.Label, MoodValue = (int)Math.Round((double)ue.MoodValue) })
                .ToListAsync();

            var diaryEmotions = await db.DiaryEmotions
                .Where(de => de.Diary.UserId == userId && de.Diary.EntryDate >= startDate)
                .Select(de => new { de.Emotion.Label, MoodValue = (int)Math.Round((double)(de.Score * 9 + 1)) })
                .ToListAsync();

            var grouped = allEmotions.Concat(diaryEmotions)
                .GroupBy(e => e.Label.ToLower())
                .ToDictionary(g => g.Key, g => (int)Math.Round(g.Average(e => e.MoodValue)));

            Console.WriteLine("[PredictBrainDamage] Grouped emotions:");
            foreach (var kvp in grouped)
            {
                Console.WriteLine($"  - {kvp.Key}: {kvp.Value}");
            }

            // Calculează media ponderată pentru fiecare emoție
            int Get(string key) => grouped.TryGetValue(key, out var val) ? val : 1;

            var user = await _userManager.GetUserAsync(User);
            Console.WriteLine($"[PredictBrainDamage] User Age: {user.Age}, Sex: {user.Sex}");

            var input = new EmotionalInputData
            {
                Age = (float)user.Age,
                Sex = user.Sex,
                Joy = Get("joy"),
                Sadness = Get("sadness"),
                Anger = Get("anger"),
                Love = Get("love"),
                Fear = Get("fear"),
                Surprise = Get("surprise"),
                Disgust = Get("disgust"),
                YearsSinceStart = request.YearsSinceStart
            };

            Console.WriteLine("[PredictBrainDamage] Input for model:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(input));

            var prediction = _predictionService.PredictBrainDamageProgression(input);

            Console.WriteLine("[PredictBrainDamage] Prediction result:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(prediction));

            return Json(prediction);
        }



        public class PredictionRequest
        {
            public DateTime Date { get; set; }
            public int YearsSinceStart { get; set; }
        }

    }
}
