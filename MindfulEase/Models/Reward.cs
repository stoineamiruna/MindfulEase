using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Reward
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string Activity { get; set; }
        public int Points { get; set; }
        public DateTime DateEarned { get; set; }

        // Relație Many-to-One cu ApplicationUser
        public virtual ApplicationUser? User { get; set; }
    }

}
