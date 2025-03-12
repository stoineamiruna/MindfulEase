using MindfulEase.Data;
using MindfulEase.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MindfulEase.Services
{
    public class WeeklyChallengeService
    {
        private readonly ApplicationDbContext _context;
        private readonly RewardService _rewardService;

        public WeeklyChallengeService(ApplicationDbContext context, RewardService rewardService)
        {
            _context = context;
            _rewardService = rewardService;
        }

        // Metodă pentru a verifica și completa challenge-urile pentru fiecare utilizator
        public async Task CheckWeeklyChallengesAsync()
        {
            var challenges = await _context.WeeklyChallenges.ToListAsync();

            foreach (var challenge in challenges)
            {
                // Verifică dacă challenge-ul este activ (între StartDate și EndDate)
                if (challenge.StartDate <= DateTime.Now && challenge.EndDate >= DateTime.Now)
                {
                    var usersInChallenge = _context.Users.ToList();  

                    foreach (var user in usersInChallenge)
                    {
                        // Verifică dacă intrarea există deja în ApplicationUserWeeklyChallenge
                        var existingEntry = await _context.ApplicationUserWeeklyChallenges
                            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.WeeklyChallengeId == challenge.Id);

                        if (existingEntry == null || !existingEntry.IsCompleted)
                        {
                            bool conditionsMet = true;

                            // Verifică punctele, dacă este necesar
                            if (challenge.RequiredPoints.HasValue)
                            {
                                var userPoints = await _context.Rewards
                                    .Where(a => a.UserId == user.Id && a.DateEarned >= challenge.StartDate && a.DateEarned <= challenge.EndDate)
                                    .SumAsync(a => a.Points);

                                if (userPoints < challenge.RequiredPoints)
                                {
                                    conditionsMet = false;
                                }
                            }

                            // Verifică jurnalul de înregistrări, dacă este necesar
                            if (challenge.RequiredJournalEntries.HasValue)
                            {
                                var journalEntries = await _context.Diaries
                                    .Where(d => d.UserId == user.Id && d.EntryDate >= challenge.StartDate && d.EntryDate <= challenge.EndDate)
                                    .CountAsync();

                                if (journalEntries < challenge.RequiredJournalEntries)
                                {
                                    conditionsMet = false;
                                }
                            }

                            // Verifică quizzurile, dacă este necesar
                            if (challenge.RequiredQuizzes.HasValue)
                            {
                                var quizzesCompleted = await _context.ApplicationUserQuizzes
                                    .Where(q => q.UserId == user.Id && q.Date >= challenge.StartDate && q.Date <= challenge.EndDate)
                                    .CountAsync();

                                if (quizzesCompleted < challenge.RequiredQuizzes)
                                {
                                    conditionsMet = false;
                                }
                            }

                            // Dacă toate condițiile sunt îndeplinite, adaugă intrarea
                            if (conditionsMet)
                            {
                                if (existingEntry == null)
                                {
                                    // Creați o intrare nouă în ApplicationUserWeeklyChallenge
                                    var newEntry = new ApplicationUserWeeklyChallenge
                                    {
                                        UserId = user.Id,
                                        WeeklyChallengeId = challenge.Id,
                                        IsCompleted = true,
                                        CompletedDate = DateTime.Now
                                    };
                                    _context.ApplicationUserWeeklyChallenges.Add(newEntry);

                                    var notification = new Notification
                                    {
                                        UserId = user.Id,
                                        Message = $"Congratulations! Challenge {challenge.Title} has been completed! You have received 20 points. ",
                                        Link = "/WeeklyChallenges/Index",
                                        CreatedAt = DateTime.Now,
                                        IsRead = false
                                    };

                                    _context.Notifications.Add(notification);

                                    await _rewardService.AddRewardAsync(user.Id, "CompleteChallenge", 20);
                                }
                                else
                                {
                                    // Marcare completă dacă există deja
                                    existingEntry.IsCompleted = true;
                                    existingEntry.CompletedDate = DateTime.Now;
                                }
                            }
                        }

                    }
                }
            }
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                var daysStreak = CalculateDaysStreak(user.Id);
                if (daysStreak > 0 && daysStreak % 25 == 0) {
                    var existingReward = _context.Rewards.FirstOrDefaultAsync(x => x.UserId == user.Id && x.DateEarned == DateTime.Now && x.Activity == "DaysStreak");
                    if(existingReward != null)
                    {
                        await _rewardService.AddRewardAsync(user.Id, "DaysStreak", 25);
                        var notification = new Notification
                        {
                            UserId = user.Id,
                            Message = $"Congratulations! You have earned 25 points for the {daysStreak} Days Streak!",
                            Link = "/ApplicationUsers/Show/" + user.Id,
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };

                        _context.Notifications.Add(notification);
                    }
                        
                }
            }

            // Salvează modificările
            await _context.SaveChangesAsync();
        }

        private int CalculateDaysStreak(string userId)
        {
            var userObjectives = _context.UserObjectives
                            .Where(uo => uo.UserId == userId)
                            .Include(uo => uo.Objective)
                            .ToList();

            // Obținem Id-urile obiectivelor utilizatorului
            var userObjectiveIds = userObjectives.Select(uo => (int?)uo.Id).ToList();

            var userObjectiveProgresses = _context.UserObjectiveProgresses
                .Where(up => userObjectiveIds.Contains(up.UserObjectiveId) && up.IsCompleted)
                .OrderByDescending(up => up.Date) // Sortăm descrescător pentru a verifica zile consecutive
                .Select(up => up.Date.Date) // Luăm doar datele unice
                .Distinct()
                .ToList();
            userObjectiveProgresses = userObjectiveProgresses.OrderByDescending(up => up.Date).ToList();
            int daysStreak = 0;
            DateTime today = DateTime.UtcNow.Date;
            foreach (var x in userObjectiveProgresses)
            {
                Console.WriteLine("userObjectiveProgresses: " + x);
            }

            Console.WriteLine("nr: " + userObjectiveProgresses.Count);

            // Verificăm dacă utilizatorul a completat obiective pentru ziua de azi
            if (!userObjectiveProgresses.Contains(today))
                today = today.AddDays(-1); // Dacă azi nu a completat, începem streak-ul de ieri


            // Parcurgem zilele consecutive
            foreach (var date in userObjectiveProgresses)
            {
                Console.WriteLine("date: " + date);
                Console.WriteLine("today: " + today);
                if (date == today)
                {
                    daysStreak++;
                    today = today.AddDays(-1); // Verificăm ziua anterioară
                }
                else
                {
                    break; // Dacă o zi lipsește, streak-ul s-a întrerupt
                }
            }

            return daysStreak;
        }
    }
}
