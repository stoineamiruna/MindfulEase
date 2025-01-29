using Microsoft.AspNetCore.Mvc;
using MindfulEase.Data;
using MindfulEase.Models;
using System.Linq;
using System.Security.Claims;

namespace MindfulEase.Controllers
{
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Dependency Injection pentru ApplicationDbContext
        public ApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetBrainData()
        {
            // Obține ID-ul utilizatorului curent
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Preia emoțiile utilizatorului și le mapăm la regiuni și culori
            var emotions = _context.DiaryEmotions
                .Where(e => e.Diary.UserId == userId)
                .Select(e => new
                {
                    Region = e.EmotionId.HasValue ? MapEmotionToRegion(e.EmotionId.Value) : "Unknown",
                    Color = e.Score.HasValue ? MapEmotionToColor(e.Score.Value) : "#808080" // Gri pentru necunoscut
                }).ToList();

            return Json(emotions);
        }

        [HttpGet]
        public IActionResult GetBrainRegionInfo(string region)
        {
            // Găsim informațiile pentru regiunea selectată
            var info = _context.BrainRegions.FirstOrDefault(r => r.Name == region);
            if (info == null)
            {
                return NotFound("Region not found.");
            }

            return Json(info);
        }

        // Mapăm emoțiile la regiuni cerebrale
        private static string MapEmotionToRegion(int emotionId)
        {
            return emotionId switch
            {
                1 => "Amigdala",
                _ => "Cortex Prefrontal"
            };
        }

        // Mapăm scorurile emoționale la culori
        private string MapEmotionToColor(double score)
        {
            if (score < 3) return "#00ff00"; // Verde
            if (score < 7) return "#ffff00"; // Galben
            return "#ff0000"; // Roșu
        }
    }
}
