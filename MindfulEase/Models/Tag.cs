using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public virtual ICollection<QuizTag>? Quizzes { get; set;}
        public virtual ICollection<ResourceTag>? Resources { get; set; }
        public virtual ICollection<TherapeuticGameTag>? TherapeuticGames { get; set; }

    }
}
