using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Reward
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public int StarsEarned { get; set; }
        public DateTime DateEarned { get; set; }

        // Relație Many-to-One cu ApplicationUser
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }

}
