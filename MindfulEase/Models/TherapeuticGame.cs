using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class TherapeuticGame
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [MaxLength(100, ErrorMessage = "Type cannot exceed 100 characters.")]
        public string Type { get; set; }  // Ex: "Puzzle", "Memory Game", etc.
        [MaxLength(2000, ErrorMessage = "Instructions cannot exceed 2000 characters.")]
        public string Instructions { get; set; }
        [MaxLength(500, ErrorMessage = "GameUrl cannot exceed 500 characters.")]
        public string GameUrl { get; set; } // URL-ul jocului extern
        [Url(ErrorMessage = "Background must be a valid URL.")]
        public string? Background { get; set; }

        // Relație Many-to-Many
        public virtual ICollection<ApplicationUserTherapeuticGame>? Users { get; set; }
        public virtual ICollection<TherapeuticGameTag>? Tags { get; set; }
    }
}
