using MindfulEase.Data;
using MindfulEase.Models;
using Microsoft.EntityFrameworkCore;


namespace MindfulEase.Services
{
    public class RewardService
    {
        private readonly ApplicationDbContext _context;

        public RewardService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acordă puncte pentru o activitate specifică
        public async Task AddRewardAsync(string userId, string activity, int points)
        {
            var reward = new Reward
            {
                UserId = userId,
                Activity = activity,
                Points = points,
                DateEarned = DateTime.UtcNow
            };

            // Adaugă reward în baza de date
            _context.Rewards.Add(reward);
            await _context.SaveChangesAsync();
        }

        // Obține totalul de puncte ale unui utilizator
        public async Task<int> GetTotalPointsAsync(string userId)
        {
            var totalPoints = await _context.Rewards
                .Where(r => r.UserId == userId)
                .SumAsync(r => r.Points);

            return totalPoints;
        }
    }
}
