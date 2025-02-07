using System.ComponentModel.DataAnnotations;
namespace MindfulEase.Models
{
    public class ApplicationUserQuestionQuiz
    {
        [Key]
        public int Id { get; set; }

        // Relația cu utilizatorul
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        // Relația cu întrebarea din quiz
        public int? QuestionId { get; set; }
        public virtual QuestionQuiz? Question { get; set; }

        // Valoarea răspunsului (1-5 sau 0-4, depinde de quiz)
        public int ResponseValue { get; set; }
    }
}
