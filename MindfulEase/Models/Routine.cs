using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Routine
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DifficultyLevel { get; set; }

        // Relație Many-to-One cu ApplicationUser
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }

}
