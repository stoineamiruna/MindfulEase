using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class TherapeuticGame
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }  // Ex: "Puzzle", "Memory Game", etc.

        public string Instructions { get; set; }

        public string GameUrl { get; set; } // URL-ul jocului extern

        // Relație Many-to-Many
        public virtual ICollection<ApplicationUserTherapeuticGame> Users { get; set; }
    }
}
