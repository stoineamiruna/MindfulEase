using MindfulEase.Data;
using MindfulEase.Models;
using Microsoft.EntityFrameworkCore;


public class BadgeService
{
    private readonly ApplicationDbContext _context;

    public BadgeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CheckAndAwardBadgesAsync()
    {
        try
        {
            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                await CheckAndAwardBadgesForUser(user.Id);
            }
        }
        catch (Exception ex)
        {
            // Loghează eroarea pentru a înțelege mai bine ce se întâmplă
            Console.WriteLine($"Eroare la rularea jobului pentru BadgeService: {ex.Message}");
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
        if (meditationSessions >= 10 && !await HasBadge(userId, "Zen Master 🧘‍♂️"))
        {
            await AwardBadge(userId, "Zen Master 🧘‍♂️");
        }

        // Exemplu pentru badge-ul "Stress Buster 🛑"
        var stressReductionScore = rewards.Where(r => r.Activity == "Quiz")
                                          .Sum(r => r.Points);
        if (stressReductionScore >= 50 && !await HasBadge(userId, "Stress Buster 🛑"))
        {
            await AwardBadge(userId, "Stress Buster 🛑");
        }

        // Exemplu pentru badge-ul "Deep Thinker ✍️"
        var journalEntries = rewards.Count(r => r.Activity == "Diary");
        if (journalEntries >= 10 && !await HasBadge(userId, "Deep Thinker ✍️"))
        {
            await AwardBadge(userId, "Deep Thinker ✍️");
        }

        // Exemplu pentru badge-ul "Emotional Explorer 🎭"
        var consecutiveReflectionDays = rewards
            .Where(r => r.Activity == "Diary")
            .GroupBy(r => r.DateEarned.Date)
            .Count();
        if (consecutiveReflectionDays >= 7 && !await HasBadge(userId, "Emotional Explorer 🎭"))
        {
            await AwardBadge(userId, "Emotional Explorer 🎭");
        }

        // Exemplu pentru badge-ul "Sleep Champion 😴"
        var consistentSleep = rewards.Count(r => r.Activity == "Sleep" && r.DateEarned.Date == DateTime.UtcNow.Date);
        if (consistentSleep >= 7 && !await HasBadge(userId, "Sleep Champion 😴"))
        {
            await AwardBadge(userId, "Sleep Champion 😴");
        }
    }

    // Verifică dacă utilizatorul are deja un badge
    private async Task<bool> HasBadge(string userId, string badgeTitle)
    {
        // Găsește badge-ul pe baza titlului
        var badge = await _context.Badges
            .FirstOrDefaultAsync(b => b.Title == badgeTitle);

        // Dacă badge-ul nu există, returnează false
        if (badge == null) return false;

        // Verifică dacă utilizatorul are acest badge
        return await _context.UserBadges
            .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badge.Id);
    }

    // Acordă badge-ul unui utilizator
    private async Task AwardBadge(string userId, string badgeTitle)
    {
        var badge = await _context.Badges
            .FirstOrDefaultAsync(b => b.Title == badgeTitle);

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
