using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MindfulEase.Data;
using MindfulEase.Models;
using System.Linq;

namespace MindfulEase.Controllers
{
    public class WeeklyReportsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public WeeklyReportsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Afișează detalii pentru un raport săptămânal specificat
        public async Task<IActionResult> Show(int id)
        {
            // Preia raportul săptămânal curent
            var report = await _db.WeeklyReports
                .Include(wr => wr.User)
                .FirstOrDefaultAsync(wr => wr.Id == id);

            if (report == null)
            {
                Console.WriteLine("Raportul cu ID-ul " + id + " nu a fost găsit.");
                return NotFound();
            }

            Console.WriteLine("Raportul a fost găsit: " + report.Id);

            // Obține emoțiile din baza de date (ordinea lor)
            var allEmotions = await _db.Emotions.ToListAsync();
            Console.WriteLine("Număr total emoții în baza de date: " + allEmotions.Count);

            // Verificăm ce conținut are AverageEmotions și îl divizăm în elemente
            if (!string.IsNullOrEmpty(report.AverageEmotions))
            {
                Console.WriteLine("AverageEmotions pentru raportul curent: " + report.AverageEmotions);
            }
            else
            {
                Console.WriteLine("Nu există emoții medii în raport.");
            }

            // Transformă media emoțiilor într-o listă de emoții
            var emotionsList = new List<dynamic>();

            foreach (var (value, index) in report.AverageEmotions.Split(';').Select((v, i) => (v, i)))
            {
                emotionsList.Add(new
                {
                    EmotionName = allEmotions.ElementAtOrDefault(index)?.Label ?? "Unknown",
                    AverageMood = double.TryParse(value, out double result) ? result : 0.0
                });
            }

            ViewBag.EmotionsList = emotionsList;


            // Verificăm dacă emotionsList are date corecte
            Console.WriteLine("Numar de emoii preluate: " + emotionsList.Count);
            foreach (var emotion in emotionsList)
            {
                Console.WriteLine($"Emoție: {emotion.EmotionName}, Media: {emotion.AverageMood}");
            }
            Console.WriteLine("tip: "+ emotionsList.GetType().Name);

            // Verifică dacă există emoții în lista respectivă
            if (emotionsList != null && emotionsList.Any())
            {
                ViewBag.EmotionsList = emotionsList;
            }
            else
            {
                ViewBag.EmotionsList = new List<dynamic>(); // Setează o listă goală în caz că nu sunt date
                Console.WriteLine("Emoțiile nu au fost adăugate corect în ViewBag.");
            }

            // Obține rapoartele săptămânale ale utilizatorului curent
            var userReports = await _db.WeeklyReports
                .Where(wr => wr.UserId == report.UserId)
                .OrderByDescending(wr => wr.WeekStartDate)
                .Take(3)  // Limităm la 3 rapoarte pentru paginare
                .ToListAsync();

            Console.WriteLine("Număr de rapoarte săptămânale ale utilizatorului: " + userReports.Count);

            // Afișează raportul curent împreună cu alte rapoarte ale utilizatorului
            ViewBag.UserReports = userReports;



            //Partea de grafic

            // Setează perioada de filtrare: între WeekStartDate și WeekStartDate + 7 zile
            DateTime startDate = report.WeekStartDate;
            DateTime endDate = startDate.AddDays(7);

            var userEmotions = await _db.ApplicationUserEmotions
                .Where(e => e.UserId == report.UserId && e.Date >= startDate && e.Date < endDate)
                .GroupBy(e => new { e.Date, e.EmotionId })
                .Select(g => new
                {
                    g.Key.EmotionId,
                    Date = g.Key.Date,
                    AvgMood = g.Average(e => e.MoodValue)
                })
                .ToListAsync();

            var diaryEmotions = await _db.DiaryEmotions
                .Where(de => de.Diary.UserId == report.UserId && de.Diary.EntryDate >= startDate && de.Diary.EntryDate < endDate)
                .GroupBy(de => new { de.Diary.EntryDate, de.EmotionId })
                .Select(g => new
                {
                    g.Key.EmotionId,
                    Date = g.Key.EntryDate,
                    AvgMood = g.Average(de => (de.Score ?? 0) * 9 + 1)
                })
                .ToListAsync();

            // Combină emoțiile din ambele tabele și calculează media
            var combinedEmotions = userEmotions
                .Union(diaryEmotions)
                .GroupBy(e => new { e.EmotionId, e.Date })
                .Select(g => new
                {
                    g.Key.EmotionId,
                    g.Key.Date,
                    AvgMood = g.Average(e => e.AvgMood)
                })
                .ToList();

            // Obține numele emoțiilor
            var emotionLabels = await _db.Emotions.ToDictionaryAsync(e => e.Id, e => e.Label);

            // Transformă datele într-un format ușor de utilizat în Chart.js
            var chartData = combinedEmotions
                .Select(e => new
                {
                    Date = e.Date.ToString("yyyy-MM-dd"),
                    EmotionName = emotionLabels.ContainsKey(e.EmotionId.GetValueOrDefault())
                        ? emotionLabels[e.EmotionId.GetValueOrDefault()]
                        : "Unknown",

                    AvgMood = e.AvgMood
                })
                .GroupBy(e => e.EmotionName)
                .ToDictionary(g => g.Key, g => g.OrderBy(e => e.Date).Select(e => new { e.Date, e.AvgMood }).ToList());

            ViewBag.ChartData = chartData;
            Console.WriteLine("chartData: " + chartData);


            //Partea de obiective 


            // Obține obiectivele setate de utilizator pe zi
            var userObjectives = await _db.UserObjectives
                .Where(uo => uo.UserId == report.UserId)
                .ToListAsync();

            // Calculează obiectivele totale pentru săptămână (presupunem că toate zilele au același număr de obiective)
            int objectivesPerDay = userObjectives.Count > 0 ? userObjectives.Count : 0;
            int totalWeeklyObjectives = objectivesPerDay * 7;

            // Calculează obiectivele nefinalizate
            int completedObjectives = report.NrObjectives;
            int uncompletedObjectives = Math.Max(totalWeeklyObjectives - completedObjectives, 0);

            // Trimite datele către ViewBag
            ViewBag.ObjectivesChartData = new
            {
                completed = completedObjectives,
                uncompleted = uncompletedObjectives
            };


            return View(report);
        }


    }

}
