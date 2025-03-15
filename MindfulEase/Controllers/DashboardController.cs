using MindfulEase.Data;
using MindfulEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

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
