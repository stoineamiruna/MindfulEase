using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Emotion
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; } // Culoarea asociată în modelul 3D

        // Relație Many-to-Many
        public virtual ICollection<ApplicationUserEmotion> Users { get; set; }
    }
}
