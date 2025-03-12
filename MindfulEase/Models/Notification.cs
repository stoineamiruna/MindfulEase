using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        // Nullable UserId - notificarea poate aparține unui utilizator sau tuturor
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        [MaxLength(500)]
        public string Link { get; set; } // Link-ul asociat notificării

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
