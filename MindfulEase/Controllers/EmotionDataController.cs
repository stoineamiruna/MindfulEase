using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;

namespace MindfulEase.Controllers
{
    public class EmotionDataController : Controller
    {
        private readonly ApplicationDbContext db;
        public EmotionDataController(ApplicationDbContext context)
        {
            db = context;
        }
        [HttpGet("user/{userId}/date/{date}/brain-activity")]
        public async Task<IActionResult> GetUserBrainActivity(string userId, DateTime date)
        {
            var selectedDate = date.Date;

            // Preluăm emoțiile din ApplicationUserEmotions (1-10)
            var userEmotionsData = await db.ApplicationUserEmotions
                .Where(ue => ue.UserId == userId && ue.Date.Date == selectedDate)
                .Select(ue => new
                {
                    EmotionId = ue.EmotionId,
                    MoodValue = (double)ue.MoodValue
                })
                .ToListAsync();

            // Preluăm emoțiile din DiaryEmotions (0-1 -> convertite la 1-10)
            var diaryEmotionsData = await db.DiaryEmotions
                .Where(de => de.Diary.UserId == userId && de.Diary.EntryDate.Date == selectedDate)
                .Select(de => new
                {
                    EmotionId = de.EmotionId,
                    MoodValue = (double)(de.Score * 9 + 1)  // Conversie la 1-10
                })
                .ToListAsync();

            // Combinăm datele din ambele surse
            var combinedEmotions = userEmotionsData.Concat(diaryEmotionsData);

            // Grupăm emoțiile după EmotionId și calculăm media scorurilor
            var averagedEmotions = combinedEmotions
                .GroupBy(e => e.EmotionId)
                .Select(group => new
                {
                    EmotionId = group.Key,
                    AverageMoodValue = group.Average(g => g.MoodValue)
                })
                .ToList();

            // Asociem emoțiile cu regiunile cerebrale
            var brainActivity = averagedEmotions
                .Join(db.EmotionBrainRegions,
                      ae => ae.EmotionId,
                      bre => bre.EmotionId,
                      (ae, bre) => new { ae.AverageMoodValue, bre.BrainRegionId })
                .Join(db.BrainRegions,
                      bre => bre.BrainRegionId,
                      br => br.Id,
                      (bre, br) => new
                      {
                          BrainRegion = br.Name,
                          Color = db.Emotions.Where(e => e.Id == bre.BrainRegionId).Select(e => e.ColorCode).FirstOrDefault(),
                          Score = bre.AverageMoodValue / 10.0 // Normalizare la [0-1]
                      })
                .GroupBy(b => b.BrainRegion)
                .Select(group => new
                {
                    BrainRegion = group.Key,
                    Color = group.First().Color ?? "#FFFFFF", // Alb implicit dacă nu există culoare
                    Score = group.Average(g => g.Score) // Media intensității activării
                })
                .ToList();

            return Ok(brainActivity);
        }
    }
}
