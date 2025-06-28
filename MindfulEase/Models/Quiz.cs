using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
        public string Title { get; set; }  // Ex: "Test de Anxietate"
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Result is required.")]
        [StringLength(1500, ErrorMessage = "Result cannot exceed 1500 characters.")]
        public string Result { get; set; }
        [StringLength(1500, ErrorMessage = "CategoryMapping cannot exceed 1500 characters.")]
        public string? CategoryMapping { get; set; } // Ex: { "Anxietate": "1,3,5,7", "Depresie": "2,4,6,8" }
        [StringLength(500, ErrorMessage = "Background value too long.")]
        public string? Background { get; set; }

        public virtual ICollection<QuestionQuiz>? Questions { get; set; }

        // Relație Many-to-Many
        public virtual ICollection<ApplicationUserQuiz>? Users { get; set; }
        public virtual ICollection<QuizTag>? Tags { get; set; }
    }
}
