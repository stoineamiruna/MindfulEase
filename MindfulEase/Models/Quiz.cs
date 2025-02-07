using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }  // Ex: "Test de Anxietate"

        public string Description { get; set; }
        public string Result { get; set; }
        public string? CategoryMapping { get; set; } // Ex: { "Anxietate": "1,3,5,7", "Depresie": "2,4,6,8" }

        public virtual ICollection<QuestionQuiz>? Questions { get; set; }

        // Relație Many-to-Many
        public virtual ICollection<ApplicationUserQuiz>? Users { get; set; }
    }
}
