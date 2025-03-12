namespace MindfulEase.Models
{
    public class ApplicationUserWeeklyChallenge
    {
        public string? UserId { get; set; }
        public int? WeeklyChallengeId { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedDate { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual WeeklyChallenge? WeeklyChallenge { get; set; }
    }
}
