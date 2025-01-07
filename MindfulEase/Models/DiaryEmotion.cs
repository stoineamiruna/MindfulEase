namespace MindfulEase.Models
{
    public class DiaryEmotion
    {
        public int? DiaryId { get; set; }
        public virtual Diary? Diary { get; set; }

        public int? EmotionId { get; set; }
        public virtual Emotion? Emotion { get; set; }

        public double? Score { get; set; }
    }
}
