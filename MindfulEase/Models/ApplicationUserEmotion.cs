using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class ApplicationUserEmotion
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; } 
        public virtual ApplicationUser? User { get; set; }
        public int? EmotionId { get; set; }
        public virtual Emotion? Emotion { get; set; }
        public DateTime Date { get; set; }
        public int MoodValue { get; set; } 
    }
}
