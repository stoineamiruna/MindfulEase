using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters.")]
        public string? Title { get; set; }
        public virtual ICollection<QuizTag>? Quizzes { get; set;}
        public virtual ICollection<ResourceTag>? Resources { get; set; }
        public virtual ICollection<TherapeuticGameTag>? TherapeuticGames { get; set; }

    }
}
