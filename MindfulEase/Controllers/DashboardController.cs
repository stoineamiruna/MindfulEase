using MindfulEase.Data;
using MindfulEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Humanizer.Localisation;

namespace MindfulEase.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Moderator,User")]
        public async Task<IActionResult> Index()
        {
            var emotionalJourneyData = await _context.ApplicationUserEmotions
                .Select(ue => new
                {
                    ue.UserId,
                    ue.Date,
                    ue.EmotionId,  // Adăugăm EmotionId pentru a putea filtra pe emoții
                    MoodValue = (double?)ue.MoodValue
                })
                .ToListAsync();

            var diaryEmotionsData = await _context.DiaryEmotions
                .Select(de => new
                {
                    UserId = de.Diary.UserId,
                    Date = de.Diary.EntryDate,
                    de.EmotionId,  // Adăugăm EmotionId pentru a putea filtra pe emoții
                    //MoodValue = (double?)5
                    MoodValue = (double?)(de.Score * 9 + 1)
                })
                .ToListAsync();

            var allEmotions = await _context.Emotions.ToListAsync(); // Obținem lista de emoții

            var combinedData = emotionalJourneyData
                .Concat(diaryEmotionsData)
                .OrderBy(data => data.Date)
                .GroupBy(data => new { data.UserId, data.Date, data.EmotionId }) // Grupare după utilizator, dată și emoție
                .Select(g => new
                {
                    g.Key.UserId,
                    g.Key.Date,
                    g.Key.EmotionId,
                    AvgMoodValue = g.Average(x => x.MoodValue) // Calculăm media
                })
                .GroupBy(data => data.UserId)
                .Select(g => new UserEmotionalJourney
                {
                    UserId = g.Key,
                    AvgMoodValue = (double)(g.Average(x => x.AvgMoodValue) ?? 0), // Media valorilor per user
                    EmotionalPath = g.Select(x => new EmotionalPathItem
                    {
                        Date = x.Date,
                        MoodValue = (int)Math.Round(x.AvgMoodValue ?? 0), // Dacă este null, folosim 0
                        EmotionId = x.EmotionId
                    }).ToList()
                })
                .ToList();


            ViewData["Emotions"] = allEmotions;  // Trimit lista de emoții către View

            ViewData["EmotionalJourneyData"] = JsonSerializer.Serialize(combinedData);

            var dashboardStats = new DashboardStats
            {
                TotalUsers = await GetTotalUsers(),
                TotalDiaries = await GetTotalDiaries(),
                TotalEmotions = await GetTotalEmotions(),
                TotalRewards = await GetTotalRewards(),
                TotalUserObjectives = await GetTotalUserObjectives(),
                TotalCompletedObjectives = await GetTotalCompletedObjectives(),
                ActiveUsers = await GetActiveUsers(),
                TopActiveUsers = await GetTopActiveUsers(),
                TrendingEmotions = await GetTrendingEmotions(),
                UserEmotionalJourney = await GetUserEmotionalJourney()
            };

            ViewData["TrendingEmotionsData"] = JsonSerializer.Serialize(
                dashboardStats.TrendingEmotions.Select(e => new { e.EmotionName, e.UserCount })
            );
            var userId = _userManager.GetUserId(User);


            var rewardProgressResult = GetRewardProgressData().Value as RewardProgressResult;
            Console.WriteLine("rewardProgressResult: " + rewardProgressResult);
            Console.WriteLine("rewardProgressResult null: " + (rewardProgressResult == null));
            if (rewardProgressResult == null ||
                (rewardProgressResult.AllUsersData.Count == 0 && rewardProgressResult.UserData.Count == 0))
            {
                ViewData["RewardProgressData"] = "{}";
            }
            else
            {
                ViewData["RewardProgressData"] = JsonSerializer.Serialize(rewardProgressResult);
            }


            ViewBag.UserCurent = userId;

            return View(dashboardStats);
        }

        [Authorize(Roles = "Admin,Moderator,User")]
        public async Task<IActionResult> PersonalTrainer()
        {
            var userId = _userManager.GetUserId(User);
            var last7Days = DateTime.UtcNow.Date.AddDays(-6);

            // Preluăm emoțiile din ApplicationUserEmotions (valori 1-10)
            var userEmotionsData = await _context.ApplicationUserEmotions
                .Where(ue => ue.UserId == userId && ue.Date >= last7Days)
                .Select(ue => new
                {
                    Date = ue.Date.Date,
                    EmotionLabel = ue.Emotion.Label,
                    EmotionId = ue.EmotionId,
                    MoodValue = (double)ue.MoodValue
                })
                .ToListAsync();

            // Preluăm emoțiile din DiaryEmotions (valori 0-1 -> convertim la 1-10)
            var diaryEmotionsData = await _context.DiaryEmotions
                .Where(de => de.Diary.UserId == userId && de.Diary.EntryDate >= last7Days)
                .Select(de => new
                {
                    Date = de.Diary.EntryDate.Date,
                    EmotionLabel = de.Emotion.Label,
                    EmotionId = de.EmotionId,
                    MoodValue = (double)(de.Score * 9 + 1)  // Conversie la 1-10
                })
                .ToListAsync();

            // Combinăm datele din ambele surse
            var combinedData = userEmotionsData.Concat(diaryEmotionsData);

            var groupedByDate = combinedData
                .GroupBy(e => e.Date)
                .Select(g =>
                {
                    double moodSum = 0;
                    int count = 0;

                    foreach (var entry in g)
                    {
                        if (new[] { "joy", "love", "surprise" }.Contains(entry.EmotionLabel.ToLower()))
                        {
                            moodSum += entry.MoodValue;
                        }
                        else
                        {
                            moodSum += 10 - entry.MoodValue; // Inversăm pentru emoții negative
                        }
                        count++;
                    }

                    return new
                    {
                        Date = g.Key.ToString("yyyy-MM-dd"),
                        MoodValue = count > 0 ? moodSum / count : 5
                    };
                })
                .OrderBy(e => e.Date)
                .ToList();

            // Calcul scor stare de bine (ponderare)
            double wellBeingScore = 0;
            double weightSum = 0;
            double weight = 1.0;

            foreach (var day in groupedByDate.OrderByDescending(e => e.Date))
            {
                wellBeingScore += day.MoodValue * weight;
                weightSum += weight;
                weight -= 0.1;
            }

            wellBeingScore = weightSum > 0 ? wellBeingScore / weightSum : 5;

            // Calculăm media ponderată pentru fiecare emoție
            var emotionScores = combinedData
                .GroupBy(e => e.EmotionLabel)
                .Select(g =>
                {
                    double emotionScoreSum = 0;
                    double emotionWeightSum = 0;
                    double emotionWeight = 1.0;

                    foreach (var entry in g.OrderByDescending(e => e.Date))
                    {
                        emotionScoreSum += entry.MoodValue * emotionWeight;
                        emotionWeightSum += emotionWeight;
                        emotionWeight -= 0.1;
                    }

                    double? avgScore = emotionWeightSum > 0 ? emotionScoreSum / emotionWeightSum : null;

                    return new
                    {
                        Emotion = g.Key,
                        AvgMoodValue = avgScore
                    };
                })
                .ToDictionary(e => e.Emotion, e => e.AvgMoodValue);

            // Lista de emoții relevante
            var relevantEmotions = new[] { "Fear", "Love", "Disgust", "Joy", "Surprise", "Sadness", "Anger", "Self-Awareness" };

            // Lista de emoții pozitive
            var positiveEmotions = new[] { "Love", "Joy", "Surprise" };

            // Dicționare pentru resurse, quizuri și jocuri terapeutice
            var resources = new List<dynamic>();
            var quizzes = new List<dynamic>();
            var therapeuticGames = new List<dynamic>();
            var resourceIds = new HashSet<int>();
            var quizIds = new HashSet<int>();
            var gameIds = new HashSet<int>();

            foreach (var emotion in relevantEmotions)
            {
                var emotionScore = emotionScores.ContainsKey(emotion.ToLower()) ? emotionScores[emotion.ToLower()] : null;

                if (emotionScore.HasValue && emotionScore >= 4)
                {
                    var adjustedScore = positiveEmotions.Contains(emotion) ? 10 - emotionScore.Value : emotionScore.Value;

                    if (adjustedScore >= 4)
                    {
                        // RESURSE
                        var emotionResources = await _context.ResourceTags
                            .Where(rt => rt.Tag.Title == emotion)
                            .Join(_context.Resources,
                                rt => rt.ResourceId,
                                r => r.Id,
                                (rt, r) => new
                                {
                                    r.Id,
                                    r.Title,
                                    r.Link,
                                    r.ResourceType
                                })
                            .ToListAsync();

                        foreach (var res in emotionResources)
                        {
                            if (resourceIds.Add(res.Id)) // adaugă doar dacă nu e deja
                                resources.Add(res);
                        }

                        // QUIZURI
                        var emotionQuizzes = await _context.QuizTags
                            .Where(qt => qt.Tag.Title == emotion)
                            .Join(_context.Quizzes,
                                qt => qt.QuizId,
                                q => q.Id,
                                (qt, q) => new
                                {
                                    q.Id,
                                    q.Title,
                                    q.Background
                                })
                            .ToListAsync();

                        foreach (var quiz in emotionQuizzes)
                        {
                            if (quizIds.Add(quiz.Id))
                                quizzes.Add(quiz);
                        }

                        // JOCURI TERAPEUTICE
                        var emotionTherapeuticGames = await _context.TherapeuticGameTags
                            .Where(tgt => tgt.Tag.Title == emotion)
                            .Join(_context.TherapeuticGames,
                                tgt => tgt.TherapeuticGameId,
                                tg => tg.Id,
                                (tgt, tg) => new
                                {
                                    tg.Id,
                                    tg.Name,
                                    tg.Background,
                                    tg.Type
                                })
                            .ToListAsync();

                        foreach (var game in emotionTherapeuticGames)
                        {
                            if (gameIds.Add(game.Id))
                                therapeuticGames.Add(game);
                        }
                    }
                }
            }



            // Completăm lista de resurse, evitând duplicatele
            if (resources.Count < 5)
            {
                var selfAwarenessResources = await _context.ResourceTags
                    .Where(rt => rt.Tag.Title == "Self-Awareness")
                    .Join(_context.Resources,
                        rt => rt.ResourceId,
                        r => r.Id,
                        (rt, r) => new
                        {
                            r.Id,
                            r.Title,
                            r.Link,
                            r.ResourceType
                        })
                    .Where(r => !resourceIds.Contains(r.Id))
                    .Take(5 - resources.Count)
                    .ToListAsync();

                foreach (var res in selfAwarenessResources)
                {
                    if (resourceIds.Add(res.Id))
                        resources.Add(res);
                }
            }

            // Completăm lista de quiz-uri, evitând duplicatele
            // Completăm lista de quiz-uri, evitând duplicatele
            if (quizzes.Count < 5)
            {
                var selfAwarenessQuizzes = await _context.QuizTags
                    .Where(qt => qt.Tag.Title == "Self-Awareness")
                    .Join(_context.Quizzes,
                        qt => qt.QuizId,
                        q => q.Id,
                        (qt, q) => new
                        {
                            q.Id,
                            q.Title,
                            q.Background
                        })
                    .Where(q => !quizIds.Contains(q.Id))
                    .Take(5 - quizzes.Count)
                    .ToListAsync();

                foreach (var quiz in selfAwarenessQuizzes)
                {
                    if (quizIds.Add(quiz.Id))
                        quizzes.Add(quiz);
                }
            }

            // Completăm lista de jocuri terapeutice, evitând duplicatele
            if (therapeuticGames.Count < 5)
            {
                var selfAwarenessTherapeuticGames = await _context.TherapeuticGameTags
                    .Where(tgt => tgt.Tag.Title == "Self-Awareness")
                    .Join(_context.TherapeuticGames,
                        tgt => tgt.TherapeuticGameId,
                        tg => tg.Id,
                        (tgt, tg) => new
                        {
                            tg.Id,
                            tg.Name,
                            tg.Background,
                            tg.Type
                        })
                    .Where(tg => !gameIds.Contains(tg.Id))
                    .Take(5 - therapeuticGames.Count)
                    .ToListAsync();

                foreach (var game in selfAwarenessTherapeuticGames)
                {
                    if (gameIds.Add(game.Id))
                        therapeuticGames.Add(game);
                }
            }


            // Stocăm datele în ViewData pentru a le accesa în view
            ViewData["Resources"] = resources;
            ViewData["Quizzes"] = quizzes;
            ViewData["TherapeuticGames"] = therapeuticGames;


            Console.WriteLine("resources: "+resources.Count);
            Console.WriteLine("quizzes: "+quizzes.Count);
            Console.WriteLine("games: "+therapeuticGames.Count);


            ViewData["MoodChartData"] = JsonSerializer.Serialize(groupedByDate);
            ViewData["WellBeingScore"] = wellBeingScore;
            ViewData["EmotionsScores"] = emotionScores; // Stocăm media ponderată a fiecărei emoții

            return View();
        }




        private async Task<int> GetTotalUsers()
        {
            return await Task.FromResult(_context.Users.Count());
        }

        private async Task<int> GetTotalDiaries()
        {
            return await _context.Diaries.CountAsync();
        }

        private async Task<int> GetTotalEmotions()
        {
            return await _context.Emotions.CountAsync();
        }

        private async Task<int> GetTotalRewards()
        {
            return await _context.Rewards.CountAsync();
        }

        private async Task<int> GetTotalUserObjectives()
        {
            return await _context.UserObjectives.CountAsync();
        }

        private async Task<int> GetTotalCompletedObjectives()
        {
            return await _context.UserObjectiveProgresses
                .Where(p => p.IsCompleted)
                .CountAsync();
        }

        private async Task<int> GetActiveUsers()
        {
            return await _context.Users
                .Where(user => _context.Diaries.Any(d => d.UserId == user.Id))
                .CountAsync();
        }

        private async Task<List<UserStats>> GetTopActiveUsers()
        {
            var userScores = await _context.Users
                .Select(user => new
                {
                    UserId = user.Id,
                    TotalPoints = _context.Rewards
                        .Where(r => r.UserId == user.Id)
                        .Sum(r => r.Points),
                    TotalActivities = _context.Diaries
                        .Where(d => d.UserId == user.Id)
                        .Count()
                })
                .OrderByDescending(u => u.TotalPoints)
                .ThenByDescending(u => u.TotalActivities)
                .Take(5)
                .ToListAsync();

            var topUsers = userScores
                .Select(u => new UserStats
                {
                    UserId = u.UserId,
                    TotalPoints = u.TotalPoints,
                    TotalActivities = u.TotalActivities,
                    Username = _context.Users.FirstOrDefault(user => user.Id == u.UserId)?.UserName ?? "N/A"
                })
                .ToList();

            return topUsers;
        }

        private async Task<List<EmotionStats>> GetTrendingEmotions()
        {
            var trendingEmotions = await _context.Emotions
                .Select(emotion => new EmotionStats
                {
                    EmotionName = emotion.Label,
                    UserCount = _context.Diaries
                        .Where(d => d.Emotions.Any(e => e.EmotionId == emotion.Id))
                        .Select(d => d.UserId)
                        .Distinct()
                        .Count()
                })
                .OrderByDescending(stat => stat.UserCount)
                .Take(5)
                .ToListAsync();

            return trendingEmotions;
        }

        private async Task<List<UserEmotionalJourney>> GetUserEmotionalJourney()
        {
            var emotionalJourney = await _context.ApplicationUserEmotions
                .GroupBy(ue => ue.UserId)
                .Select(g => new UserEmotionalJourney
                {
                    UserId = g.Key,
                    AvgMoodValue = g.Average(ue => ue.MoodValue),
                    EmotionalPath = g.OrderBy(ue => ue.Date).Select(ue => new EmotionalPathItem
                    {
                        Date = ue.Date,
                        MoodValue = ue.MoodValue
                    }).ToList()
                })
                .ToListAsync();

            return emotionalJourney;
        }

        [HttpGet]
        public JsonResult GetRewardProgressData()
        {
            var userId = _userManager.GetUserId(User);

            var rewardData = _context.Rewards
                .GroupBy(r => new { r.UserId, Date = r.DateEarned.Date })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    Date = g.Key.Date,
                    TotalPoints = g.Sum(r => r.Points)
                })
                .ToList();

            var allUsersData = rewardData
                .GroupBy(r => r.Date)
                .Select(g => new RewardProgressData
                {
                    TimePeriod = g.Key.ToString("yyyy-MM-dd"),
                    AvgPoints = g.Average(r => r.TotalPoints)
                })
                .OrderBy(r => r.TimePeriod)
                .ToList();

            var userData = rewardData
                .Where(r => r.UserId == userId)
                .GroupBy(r => r.Date)
                .Select(g => new RewardProgressData
                {
                    TimePeriod = g.Key.ToString("yyyy-MM-dd"),
                    AvgPoints = g.Sum(r => r.TotalPoints)
                })
                .OrderBy(r => r.TimePeriod)
                .ToList();

            return Json(new RewardProgressResult { AllUsersData = allUsersData, UserData = userData });
        }





    }

    // Dashboard Statistics Model
    public class DashboardStats
    {
        public int TotalUsers { get; set; }
        public int TotalDiaries { get; set; }
        public int TotalEmotions { get; set; }
        public int TotalRewards { get; set; }
        public int TotalUserObjectives { get; set; }
        public int TotalCompletedObjectives { get; set; }
        public int ActiveUsers { get; set; }
        public double CompletedObjectivesProportion { get; set; }
        public List<UserStats> TopActiveUsers { get; set; } = new();
        public List<EmotionStats> TrendingEmotions { get; set; } = new();
        public List<UserEmotionalJourney> UserEmotionalJourney { get; set; } = new();
    }

    // User Statistics Model
    public class UserStats
    {
        public string UserId { get; set; }
        public int TotalPoints { get; set; }
        public int TotalActivities { get; set; }
        public string Username { get; set; }
    }

    // Emotion Stats for trending emotions
    public class EmotionStats
    {
        public string EmotionName { get; set; }
        public int UserCount { get; set; }
    }

    public class UserEmotionalJourney
    {
        public string UserId { get; set; }
        public double AvgMoodValue { get; set; }
        public List<EmotionalPathItem> EmotionalPath { get; set; } // Schimbăm tipul la lista de EmotionalPathItem
    }

    public class EmotionalPathItem
    {
        public DateTime Date { get; set; }
        public int MoodValue { get; set; }

        public int? EmotionId { get; set; }
    }

    public class RewardProgressData
    {
        public string TimePeriod { get; set; }
        public double AvgPoints { get; set; }
    }

    public class RewardProgressResult
    {
        public List<RewardProgressData> AllUsersData { get; set; }
        public List<RewardProgressData> UserData { get; set; }
    }

}
