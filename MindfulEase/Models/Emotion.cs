using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Emotion
    {
        [Key]
        public int Id { get; set; }
        public string Label { get; set; }
        public string? ColorCode { get; set; } // Culoarea asociată în modelul 3D

        // Relație Many-to-Many
        public virtual ICollection<DiaryEmotion>? Diaries { get; set; }
        public virtual ICollection<ApplicationUserEmotion>? Users { get; set; }
        public virtual ICollection<EmotionBrainRegion>? BrainRegions { get; set; }
    }
}
