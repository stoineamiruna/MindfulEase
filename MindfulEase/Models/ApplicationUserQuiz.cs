using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class ApplicationUserQuiz
    {

        [Required]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public int? QuizId { get; set; }

        public virtual Quiz? Quiz { get; set; }

        public int TotalScore { get; set; }  // Se va calcula după completarea quiz-ului

        public bool IsCompleted { get; set; }  // Indică dacă utilizatorul a terminat quiz-ul
    }
}
