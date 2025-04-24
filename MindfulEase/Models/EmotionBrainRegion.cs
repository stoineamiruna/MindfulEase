namespace MindfulEase.Models
{
    public class EmotionBrainRegion
    {
        public int? EmotionId { get; set; }
        public int? BrainRegionId { get; set; }
        public virtual Emotion? Emotion { get; set; }
        public virtual BrainRegion? BrainRegion { get; set; }
    }
}
