using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;
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
        public VisualizeController(ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager, 
           SignInManager<ApplicationUser> signInManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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
    }
}
