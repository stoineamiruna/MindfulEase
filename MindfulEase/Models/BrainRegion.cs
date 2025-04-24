using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class BrainRegion
    {
        [Key]
        public int Id { get; set; }
        public string Name {  get; set; }
        public virtual ICollection<EmotionBrainRegion>? Emotions { get; set; }
    }
}
