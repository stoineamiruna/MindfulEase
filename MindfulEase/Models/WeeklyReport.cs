using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class WeeklyReport
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public string Summary { get; set; }

        // Relație Many-to-One cu ApplicationUser
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }

}
