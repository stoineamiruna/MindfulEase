namespace MindfulEase.Models
{
    public class ApplicationUserEmotion
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int EmotionId { get; set; }
        public virtual Emotion Emotion { get; set; }
    }
}
