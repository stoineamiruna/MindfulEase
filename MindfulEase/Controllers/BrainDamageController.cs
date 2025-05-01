using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Models.ML;
using MindfulEase.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MindfulEase.Controllers
{
    public class BrainDamageController : Controller
    {
        private readonly BrainDamagePredictionService _predictionService;
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BrainDamageController> _logger;

        public BrainDamageController(
            BrainDamagePredictionService predictionService,
            IConfiguration configuration,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<BrainDamageController> logger)
        {
            _predictionService = predictionService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        // GET: BrainDamage
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var model = new BrainDamage
            {
                Age = 25,
                Sex = "M",
                Joy = 5,
                Sadness = 5,
                Anger = 5,
                Love = 5,
                Fear = 5,
                Surprise = 5,
                Disgust = 5,
                YearsSinceStart = 5,
                Emotions = GetEmotions(),
                BrainRegions = GetBrainRegions(),
                EmotionBrainRegions = GetEmotionBrainRegions()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PredictDamage(BrainDamage model)
        {
            _logger.LogInformation("PredictDamage: Started with model {@ModelSummary}",
                new { model.Age, model.Sex, model.YearsSinceStart });

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userEmotionalHistory = await GetUserEmotionalHistory(userId);
                    var user = await _userManager.FindByIdAsync(userId);
                    var userProfile = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

                    // Modificat: Predicție pe baza istoricului și modelului multi-regiune
                    var damageProgression = _predictionService.CalculateDamageProgressionFromUserHistory(
                        userEmotionalHistory,
                        userProfile?.Age ?? model.Age,
                        userProfile?.Sex ?? model.Sex,
                        model.YearsSinceStart
                    );

                    model.PredictedDamage = damageProgression;
                    _logger.LogInformation("PredictDamage: Successfully predicted damage progression with {ResultCount} results",
                        damageProgression?.Count ?? 0);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "PredictDamage: Error occurred during prediction");
                    ModelState.AddModelError("", "A apărut o eroare la calcularea predicției: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("PredictDamage: ModelState invalid: {Errors}",
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
            }

            model.Emotions = GetEmotions();
            model.BrainRegions = GetBrainRegions();
            model.EmotionBrainRegions = GetEmotionBrainRegions();

            return View("Index", model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult TestPredict()
        {
            var inputs = new List<EmotionalInputData>
    {
        new EmotionalInputData { Age = 30, Sex = "F", Joy = 1, Sadness = 10, Anger = 10, Love = 1, Fear = 10, Surprise = 1, Disgust = 10, YearsSinceStart = 0 },
        new EmotionalInputData { Age = 30, Sex = "F", Joy = 1, Sadness = 10, Anger = 10, Love = 1, Fear = 10, Surprise = 1, Disgust = 10, YearsSinceStart = 2 },
        new EmotionalInputData { Age = 30, Sex = "F", Joy = 1, Sadness = 10, Anger = 10, Love = 1, Fear = 10, Surprise = 1, Disgust = 10, YearsSinceStart = 4 },
        new EmotionalInputData { Age = 30, Sex = "F", Joy = 1, Sadness = 10, Anger = 10, Love = 1, Fear = 10, Surprise = 1, Disgust = 10, YearsSinceStart = 6 },
        new EmotionalInputData { Age = 30, Sex = "F", Joy = 1, Sadness = 10, Anger = 10, Love = 1, Fear = 10, Surprise = 1, Disgust = 10, YearsSinceStart = 8 },
        new EmotionalInputData { Age = 30, Sex = "F", Joy = 1, Sadness = 10, Anger = 10, Love = 1, Fear = 10, Surprise = 1, Disgust = 10, YearsSinceStart = 10 },
        new EmotionalInputData { Age = 25, Sex = "M", Joy = 8, Sadness = 2, Anger = 1, Love = 9, Fear = 2, Surprise = 4, Disgust = 1, YearsSinceStart = 10 },
        new EmotionalInputData { Age = 40, Sex = "F", Joy = 5, Sadness = 5, Anger = 5, Love = 5, Fear = 5, Surprise = 5, Disgust = 5, YearsSinceStart = 10 },
        new EmotionalInputData { Age = 18, Sex = "M", Joy = 4, Sadness = 3, Anger = 3, Love = 2, Fear = 1, Surprise = 10, Disgust = 8, YearsSinceStart = 0 },
        new EmotionalInputData { Age = 18, Sex = "M", Joy = 4, Sadness = 3, Anger = 3, Love = 2, Fear = 1, Surprise = 10, Disgust = 8, YearsSinceStart = 2 },
        new EmotionalInputData { Age = 18, Sex = "M", Joy = 4, Sadness = 3, Anger = 3, Love = 2, Fear = 1, Surprise = 10, Disgust = 8, YearsSinceStart = 4 },
        new EmotionalInputData { Age = 18, Sex = "M", Joy = 4, Sadness = 3, Anger = 3, Love = 2, Fear = 1, Surprise = 10, Disgust = 8, YearsSinceStart = 6 },
        new EmotionalInputData { Age = 18, Sex = "M", Joy = 4, Sadness = 3, Anger = 3, Love = 2, Fear = 1, Surprise = 10, Disgust = 8, YearsSinceStart = 8 }
    };

            var results = new List<object>();

            foreach (var input in inputs)
            {
                var prediction = _predictionService.PredictBrainDamageProgression(input);
                results.Add(new
                {
                    Input = input,
                    Prediction = prediction
                });
            }

            return Ok(results);
        }

        private async Task<List<UserEmotionData>> GetUserEmotionalHistory(string userId)
        {
            // Extrage date despre emoțiile utilizatorului din baza de date
            var userEmotionsData = await _context.ApplicationUserEmotions
                .Where(ue => ue.UserId == userId)
                .Select(ue => new UserEmotionData
                {
                    Date = ue.Date.Date,
                    EmotionLabel = ue.Emotion.Label.ToLower(),
                    MoodValue = (double)ue.MoodValue
                })
                .ToListAsync();

            var diaryEmotionsData = await _context.DiaryEmotions
                .Where(de => de.Diary.UserId == userId)
                .Select(de => new UserEmotionData
                {
                    Date = de.Diary.EntryDate.Date,
                    EmotionLabel = de.Emotion.Label.ToLower(),
                    MoodValue = (double)(de.Score * 9 + 1)
                })
                .ToListAsync();

            // Combină toate sursele de date despre emoții
            var allEmotions = userEmotionsData.Concat(diaryEmotionsData).ToList();

            return allEmotions;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult TrainModel()
        {
            try
            {
                _predictionService.TrainModel();
                TempData["Message"] = "Modelul a fost antrenat cu succes!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Eroare la antrenarea modelului: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        private List<EmotionData> GetEmotions()
        {
            var emotions = new List<EmotionData>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Label, ColorCode FROM dbo.Emotions", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        emotions.Add(new EmotionData
                        {
                            Id = reader.GetInt32(0),
                            Label = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            ColorCode = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                        });
                    }
                }
            }

            return emotions;
        }

        private List<BrainRegionData> GetBrainRegions()
        {
            var regions = new List<BrainRegionData>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Name FROM dbo.BrainRegions", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        regions.Add(new BrainRegionData
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                        });
                    }
                }
            }

            return regions;
        }

        private List<EmotionBrainRegionData> GetEmotionBrainRegions()
        {
            var relationships = new List<EmotionBrainRegionData>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT EmotionId, BrainRegionId FROM dbo.EmotionBrainRegions", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                        {
                            relationships.Add(new EmotionBrainRegionData
                            {
                                EmotionId = reader.GetInt32(0),
                                BrainRegionId = reader.GetInt32(1)
                            });
                        }
                    }
                }
            }

            return relationships;
        }
        [HttpPost]
        [Route("BrainDamage/PredictAjax")]
        [IgnoreAntiforgeryToken] // doar dacă nu folosești AntiForgeryToken în formă
        public IActionResult PredictAjax([FromBody] EmotionalInputData input)
        {
            _logger.LogInformation("PredictAjax called with input: {@InputData}",
                new { input.Age, input.Sex, input.Joy, input.Sadness, input.Anger, input.Love, input.Fear, input.Surprise, input.Disgust, input.YearsSinceStart });

            if (input == null)
            {
                _logger.LogWarning("PredictAjax: Received null input");
                return BadRequest("Input data cannot be null");
            }

            try
            {
                var prediction = _predictionService.PredictBrainDamageProgression(input);

                if (prediction == null)
                {
                    _logger.LogWarning("PredictAjax: PredictionService returned null prediction");
                    return NotFound("Predicție invalidă sau nu s-a putut calcula");
                }

                // Debug pentru fiecare proprietate din predicție
                var debugValues = typeof(BrainRegionPrediction).GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetValue(prediction));
                _logger.LogInformation("PredictAjax: Prediction results: {@PredictionResults}", debugValues);

                // Transformă rezultatul într-un dicționar simplu { regiune: scor }
                var result = typeof(BrainRegionPrediction).GetProperties()
                    .ToDictionary(p => p.Name, p => (float)(p.GetValue(prediction) ?? 0f));

                Response.ContentType = "application/json";

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PredictAjax: Error during prediction");
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

    }
}