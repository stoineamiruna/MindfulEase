using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        // Relație Many-to-Many
        public virtual ICollection<ApplicationUserQuiz> Users { get; set; }
    }
}
