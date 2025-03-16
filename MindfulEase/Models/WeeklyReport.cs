using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class WeeklyReport
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public int NrWeeklyChallenges { get; set; }
        public int NrObjectives { get; set; }
        public int NrDiaries { get; set; }
        public string? AverageEmotions { get; set; } // Ex: "2.3;0;2.1"
        public virtual ApplicationUser? User { get; set; }
    }

}
