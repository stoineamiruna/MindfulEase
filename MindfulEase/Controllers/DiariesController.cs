using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Services;
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
        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteModerator = User.IsInRole("Moderator");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        // Show a single diary entry
        [Authorize(Roles = "Admin,User")]
        public IActionResult Show(int id)
        {
            SetAccessRights();
            var diary = db.Diaries
                .Include(d => d.User)
                .Include(d => d.Emotions) // Include legătura cu DiaryEmotion
                .ThenInclude(de => de.Emotion) // Include emoția asociată
                .FirstOrDefault(d => d.Id == id);

            if (diary.UserId != _userManager.GetUserId(User) && User.IsInRole("Admin") == false)
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
            if (diary == null)
            {
                TempData["message"] = "Diary entry not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            return View(diary);
        }

        // Display the create diary form
        [Authorize(Roles = "Admin,User")]
        public IActionResult New()
        {
            SetAccessRights();
            var userId = _userManager.GetUserId(User);
            if (userId == null)
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
            var userDiaries = db.Diaries.Where(d => d.UserId == userId).ToList(); 
            ViewBag.UserDiaries = userDiaries; 
            return View();
        }

        // Create a new diary entry
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> New(Diary newDiary)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
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

                // Actualizăm clusterizarea utilizatorilor
                var clusteringService = new ClusteringService(db);
                clusteringService.AssignClustersToUsers();

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
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var diary = db.Diaries.FirstOrDefault(d => d.Id == id);
     
            if (diary.UserId != _userManager.GetUserId(User) && User.IsInRole("Admin") == false)
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
            if (diary == null)
            {
                TempData["message"] = "Diary entry not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            return View(diary);
        }

        // Save the changes to a diary entry
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public IActionResult Edit(int id, Diary updatedDiary)
        {
            var diary = db.Diaries.FirstOrDefault(d => d.Id == id);
            if (diary.UserId != _userManager.GetUserId(User) && User.IsInRole("Admin") == false)
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
            if (diary == null)
            {
                TempData["message"] = "Diary entry not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            diary.Content = updatedDiary.Content; // Update the content of the diary
            diary.EntryDate =  updatedDiary.EntryDate;

            db.SaveChanges();

            TempData["message"] = "Diary entry updated successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("New");
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var diary = db.Diaries.FirstOrDefault(d => d.Id == id);
           
            if (diary.UserId != _userManager.GetUserId(User) && User.IsInRole("Admin") == false)
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

            if (diary == null)
            {
                TempData["message"] = "Diary entry not found.";
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
