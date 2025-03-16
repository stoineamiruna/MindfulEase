using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;

namespace MindfulEase.Services
{
    public class WeeklyReportService
    {
        private readonly ApplicationDbContext _context;

        public WeeklyReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GenerateWeeklyReports()
        {
            Console.WriteLine("GenerateWeeklyReports");
            Console.WriteLine("Data de azi: "+ DateTime.UtcNow.Date);
            DateTime endDate = DateTime.UtcNow.Date;
            Console.WriteLine("startDate: "+ DateTime.UtcNow.Date);
            DateTime startDate = endDate.AddDays(-7);
            Console.WriteLine("endDate: "+ DateTime.UtcNow.Date);

            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                Console.WriteLine("userId: " + user.Id);
                // Verifica daca raportul pentru aceasta saptamana deja exista
                bool exists = await _context.WeeklyReports
                    .AnyAsync(wr => wr.UserId == user.Id && wr.WeekStartDate == startDate);
                var existingReport = _context.WeeklyReports
                    .Where(wr => wr.UserId == user.Id && wr.WeekStartDate == startDate)
                    .FirstOrDefault();
                Console.WriteLine("exists: " + exists);
                if (exists && existingReport != null) continue;

                // Obtinem toate obiectivele utilizatorului
                var userObjectives = await _context.UserObjectives
                    .Where(uo => uo.UserId == user.Id)
                    .ToListAsync();

                var userObjectiveIds = userObjectives.Select(uo => (int?)uo.Id).ToList();

                // Obtinem progresul pentru obiectivele utilizatorului in intervalul saptamanii
                var userObjectiveProgressesCompleted = await _context.UserObjectiveProgresses
                    .Where(up => userObjectiveIds.Contains(up.UserObjectiveId) && up.Date >= startDate && up.Date < endDate && up.IsCompleted)
                    .ToListAsync();

                int completedObjectives = userObjectiveProgressesCompleted.Count;

                // Obtinem numarul de challenge-uri finalizate
                int completedChallenges = await _context.ApplicationUserWeeklyChallenges
                    .Where(c => c.UserId == user.Id && c.CompletedDate >= startDate && c.CompletedDate < endDate)
                    .CountAsync();

                Console.WriteLine("completedChallenges: " + completedChallenges);

                // Obtinem numarul de jurnale scrise
                int nrDiaries = await _context.Diaries
                    .Where(d => d.UserId == user.Id && d.EntryDate >= startDate && d.EntryDate < endDate)
                    .CountAsync();
                Console.WriteLine("nrDiaries: " + nrDiaries);

                // Calculează media emoțiilor combinate din UserEmotions și DiaryEmotion
                // Obține lista completă de emoții din baza de date (toate emoțiile posibile)
                var allEmotions = await _context.Emotions.ToListAsync();

                // Calculează media emoțiilor combinate din UserEmotions și DiaryEmotion
                var userEmotions = await _context.ApplicationUserEmotions
                    .Where(e => e.UserId == user.Id && e.Date >= startDate && e.Date < endDate)
                    .GroupBy(e => e.EmotionId)
                    .Select(g => new
                    {
                        EmotionId = g.Key,
                        AvgMood = (double?)(g.Average(e => e.MoodValue))
                    })
                    .ToListAsync();

                var diaryEmotions = await _context.DiaryEmotions
                    .Where(de => de.Diary.UserId == user.Id && de.Diary.EntryDate >= startDate && de.Diary.EntryDate < endDate)
                    .GroupBy(de => de.EmotionId)
                    .Select(g => new
                    {
                        EmotionId = g.Key,
                        AvgMood = (double?)(g.Average(de => (de.Score ?? 0) * 9 + 1))
                    })
                    .ToListAsync();

                // Combină cele două liste de emoții
                var combinedEmotions = userEmotions
                    .Union(diaryEmotions) // Folosește Union pentru a combina cele două liste
                    .GroupBy(e => e.EmotionId)
                    .Select(g => new { EmotionId = g.Key, AvgMood = g.Average(e => e.AvgMood ?? 0) })
                    .ToList();

                // Creează un dicționar cu toate emoțiile (cu media lor sau 0.0 dacă nu au raportat nimic)
                var emotionDict = allEmotions.ToDictionary(
                    e => e.Id,
                    e => combinedEmotions.FirstOrDefault(c => c.EmotionId == e.Id)?.AvgMood ?? 0.0
                );

                // Crează string-ul de medii de emoții, respectând ordinea emoțiilor din baza de date
                string averageEmotions = string.Join(";", emotionDict.Values.Select(e => e.ToString("0.0")));

                Console.WriteLine("averageEmotions: " + averageEmotions);

                // Creeaza si salveaza raportul
                var report = new WeeklyReport
                {
                    UserId = user.Id,
                    WeekStartDate = startDate,
                    NrObjectives = completedObjectives,
                    NrWeeklyChallenges = completedChallenges,
                    NrDiaries = nrDiaries,
                    AverageEmotions = averageEmotions
                };

                _context.WeeklyReports.Add(report);

                await _context.SaveChangesAsync();

                // Preluăm ID-ul raportului curent generat
                var reportId = report.Id;

                // Crează notificarea
                var notification = new Notification
                {
                    UserId = user.Id,
                    Message = $"A new weekly report is now available. Check it out for the latest updates!",
                    Link = $"/WeeklyReports/Show/{reportId}",  
                    IsRead = false
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("S-a executat");
        }
    }
}
