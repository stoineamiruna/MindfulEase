using Microsoft.AspNetCore.Mvc;
using MindfulEase.Data;
using MindfulEase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MindfulEase.Services
{
    public class BadgeService 
    {
        private readonly ApplicationDbContext _context;

        public BadgeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CheckAndAwardBadgesAsync()
        {
            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                await CheckAndAwardBadgesForUser(user.Id);
            }
        }
        // Metoda care va verifica activitățile și va acorda badge-uri
        public async Task CheckAndAwardBadgesForUser(string userId)
        {
            var rewards = await _context.Rewards
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.DateEarned)
                .ToListAsync();

            // Exemplu pentru badge-ul "Zen Master 🧘‍♂️"
            var meditationSessions = rewards.Count(r => r.Activity == "Meditatie");
            if (meditationSessions >= 10 && !await HasBadge(userId, "Zen Master"))
            {
                var badge = await _context.Badges
                    .FirstOrDefaultAsync(b => b.Title == "Zen Master 🧘‍♂️");

                if (badge != null)
                {
                    var userBadge = new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id,
                        DateUnlocked = DateTime.UtcNow
                    };

                    _context.UserBadges.Add(userBadge);
                    await _context.SaveChangesAsync();
                }
            }

            // Exemplu pentru badge-ul "Stress Buster 🛑"
            var stressReductionScore = rewards.Where(r => r.Activity == "Quiz")
                                              .Sum(r => r.Points);
            if (stressReductionScore >= 50 && !await HasBadge(userId, "Stress Buster"))
            {
                var badge = await _context.Badges
                    .FirstOrDefaultAsync(b => b.Title == "Stress Buster 🛑");

                if (badge != null)
                {
                    var userBadge = new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id,
                        DateUnlocked = DateTime.UtcNow
                    };

                    _context.UserBadges.Add(userBadge);
                    await _context.SaveChangesAsync();
                }
            }

            // Exemplu pentru badge-ul "Deep Thinker ✍️"
            var journalEntries = rewards.Count(r => r.Activity == "Diary");
            if (journalEntries >= 10 && !await HasBadge(userId, "Deep Thinker"))
            {
                var badge = await _context.Badges
                    .FirstOrDefaultAsync(b => b.Title == "Deep Thinker ✍️");

                if (badge != null)
                {
                    var userBadge = new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id,
                        DateUnlocked = DateTime.UtcNow
                    };

                    _context.UserBadges.Add(userBadge);
                    await _context.SaveChangesAsync();
                }
            }

            // Exemplu pentru badge-ul "Emotional Explorer 🎭"
            var consecutiveReflectionDays = rewards
                .Where(r => r.Activity == "Diary")
                .GroupBy(r => r.DateEarned.Date)
                .Count();

            if (consecutiveReflectionDays >= 7 && !await HasBadge(userId, "Emotional Explorer"))
            {
                var badge = await _context.Badges
                    .FirstOrDefaultAsync(b => b.Title == "Emotional Explorer 🎭");

                if (badge != null)
                {
                    var userBadge = new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id,
                        DateUnlocked = DateTime.UtcNow
                    };

                    _context.UserBadges.Add(userBadge);
                    await _context.SaveChangesAsync();
                }
            }

            // Exemplu pentru badge-ul "Sleep Champion 😴"
            var consistentSleep = rewards.Count(r => r.Activity == "Sleep" && r.DateEarned.Date == DateTime.UtcNow.Date);
            if (consistentSleep >= 7 && !await HasBadge(userId, "Sleep Champion"))
            {
                var badge = await _context.Badges
                    .FirstOrDefaultAsync(b => b.Title == "Sleep Champion 😴");

                if (badge != null)
                {
                    var userBadge = new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id,
                        DateUnlocked = DateTime.UtcNow
                    };

                    _context.UserBadges.Add(userBadge);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // Verifică dacă utilizatorul are deja un badge
        private async Task<bool> HasBadge(string userId, string badgeTitle)
        {
            var badge = await _context.Badges
                .FirstOrDefaultAsync(b => b.Title == badgeTitle);

            if (badge == null) return false;

            return await _context.UserBadges
                .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badge.Id);
        }
    }
}
