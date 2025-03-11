namespace MindfulEase.Models
{
    public class UserBadge
    {
        public int Id { get; set; }
        public string? UserId { get; set; } 
        public int? BadgeId { get; set; } 
        public DateTime DateUnlocked { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Badge? Badge { get; set; }
    }
}
