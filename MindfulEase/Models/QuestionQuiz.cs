using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class QuestionQuiz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }  // Ex: "Mă simt anxios în situații sociale."
        public bool IsReversed { get; set; }

        public int? QuizId { get; set; }
        public int Order { get; set; }
        public virtual Quiz? Quiz { get; set; }
        public virtual ICollection<ApplicationUserQuestionQuiz>? UserQuestionQuizzes { get; set; }

    }

}
