using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MindfulEase.Validation;

namespace MindfulEase.Models
{
    public class Diary
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Entry date is required.")]
        [DataType(DataType.Date)]
        [DateNotInFuture(ErrorMessage = "The date cannot be in the future.")]
        public DateTime EntryDate { get; set; }
        [Required(ErrorMessage = "The content is required.")]
        [MaxLength(1500, ErrorMessage = "Content can't exceed 1500 characters.")]
        public string Content { get; set; } // Conținutul jurnalului

        // Relație Many-to-One cu ApplicationUser
        public virtual ApplicationUser? User { get; set; }
        // Relație Many-to-Many cu ApplicationUser
        public ICollection<DiaryEmotion>? Emotions { get; set; }
    }

}
