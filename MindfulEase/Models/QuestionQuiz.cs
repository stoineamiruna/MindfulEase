using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class QuestionQuiz
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Text is required.")]
        [StringLength(300, ErrorMessage = "Text cannot exceed 300 characters.")]
        public string Text { get; set; }  // Ex: "Mă simt anxios în situații sociale."
        [Required(ErrorMessage = "Text is required.")]
        public bool IsReversed { get; set; }

        public int? QuizId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Order must be 0 or a positive integer.")]
        public int Order { get; set; }
        public virtual Quiz? Quiz { get; set; }
        public virtual ICollection<ApplicationUserQuestionQuiz>? UserQuestionQuizzes { get; set; }

    }

}
