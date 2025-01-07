using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Services.MindfulEase.Services;

namespace MindfulEase.Controllers
{
    public class DiariesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SentimentAnalysisService _sentimentAnalysisService;

        public DiariesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SentimentAnalysisService sentimentAnalysisService)
        {
            db = context;
            _userManager = userManager;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        // Show a single diary entry
        public IActionResult Show(int id)
        {
            var diary = db.Diaries
                .Include(d => d.User)
                .Include(d => d.Emotions) // Include legătura cu DiaryEmotion
                .ThenInclude(de => de.Emotion) // Include emoția asociată
                .FirstOrDefault(d => d.Id == id);
            if (diary == null)
            {
                TempData["message"] = "Diary entry not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            return View(diary);
        }

        // Display the create diary form
        public IActionResult New()
        {
            var userId = _userManager.GetUserId(User); 
            var userDiaries = db.Diaries.Where(d => d.UserId == userId).ToList(); 
            ViewBag.UserDiaries = userDiaries; 
            return View();
        }

        // Create a new diary entry
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> New(Diary newDiary)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                newDiary.UserId = userId;
                newDiary.Content = newDiary.Content;
                newDiary.EntryDate = newDiary.EntryDate;

                db.Diaries.Add(newDiary);
                await db.SaveChangesAsync();

                // Analizează emoțiile folosind serviciul de sentiment analysis
                var emotions = await _sentimentAnalysisService.AnalyzeEmotionAsync(newDiary.Content);
                Console.WriteLine("emotion: " + emotions);
                if (emotions != null)
                {
                    if (emotions != null)
                    {
                        // Iterăm prin lista de emoții
                        foreach (var emotion in emotions)
                        {
                            // Verificăm dacă elementul poate fi accesat ca un dicționar
                            if (emotion is IDictionary<string, object> emotionDict)
                            {
                
                                //var emotionInDb = await db.Emotions.FirstOrDefaultAsync(e => e.Label == emotion.label);
                                // Accesăm valorile cheilor din dicționar
                                if (emotionDict.TryGetValue("label", out var label) &&
                                    emotionDict.TryGetValue("score", out var score))
                                {
                                    Console.WriteLine($"1Label: {label}, Score: {score}");
                                }
                            }
                            else
                            {
                                string label = emotion.label?.ToString();
                                double score = Convert.ToDouble(emotion.score);
                                // Dacă este de tip dynamic, accesăm direct proprietățile
                                Console.WriteLine($"2Label: {emotion.label}, Score: {emotion.score}");
                                Console.WriteLine($"2Label: {emotion.label.GetType()}, Score: {emotion.score.GetType()}");
                                Console.WriteLine($"2Label: {label}, Score: {score}");

                                var emotionInDb = await db.Emotions.FirstOrDefaultAsync(e => e.Label == label);
                                if (emotionInDb == null)
                                {
                                    // Dacă emoția nu există în DB, o adăugăm
                                    emotionInDb = new Emotion {};
                                    emotionInDb.Label = label;
                                    db.Emotions.Add(emotionInDb);
                                    await db.SaveChangesAsync();
                                }

                                // Creăm legătura între jurnal și emoție
                                var diaryEmotion = new DiaryEmotion
                                {
                                    DiaryId = newDiary.Id,
                                    EmotionId = emotionInDb.Id,
                                    Score = score  // Asociem scorul emoției
                                };

                                db.DiaryEmotions.Add(diaryEmotion);
                                await db.SaveChangesAsync();
                            }
                        }
                    }

                }

                TempData["message"] = "Diary entry created successfully!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("New");
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(newDiary);
        }




        // Display the edit form for a diary entry
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var diary = db.Diaries.FirstOrDefault(d => d.Id == id);
            if (diary == null || diary.UserId != _userManager.GetUserId(User))
            {
                TempData["message"] = "Diary entry not found or you do not have permission to edit it.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            return View(diary);
        }

        // Save the changes to a diary entry
        [HttpPost]
        public IActionResult Edit(int id, Diary updatedDiary)
        {
            var diary = db.Diaries.FirstOrDefault(d => d.Id == id);
            if (diary == null || diary.UserId != _userManager.GetUserId(User))
            {
                TempData["message"] = "Diary entry not found or you do not have permission to edit it.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("New");
            }

            diary.Content = updatedDiary.Content; // Update the content of the diary
            diary.EntryDate =  updatedDiary.EntryDate;

            db.SaveChanges();

            TempData["message"] = "Diary entry updated successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("New");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var diary = db.Diaries.FirstOrDefault(d => d.Id == id);
            if (diary == null || diary.UserId != _userManager.GetUserId(User))
            {
                TempData["message"] = "Diary entry not found or you do not have permission to delete it.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("New");
            }

            db.Diaries.Remove(diary);
            db.SaveChanges();

            TempData["message"] = "Diary entry deleted successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("New");
        }
    }
}
