using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Diary
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime EntryDate { get; set; }
        [Required(ErrorMessage = "The content is required.")]
        public string Content { get; set; } // Conținutul jurnalului

        // Relație Many-to-One cu ApplicationUser
        public virtual ApplicationUser? User { get; set; }
        // Relație Many-to-Many cu ApplicationUser
        public ICollection<DiaryEmotion>? Emotions { get; set; }
    }

}
