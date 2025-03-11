using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class UserObjective
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }  
        public int? ObjectiveId { get; set; }  
        public int? TargetValue { get; set; }  
        public TimeSpan? TargetTime { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Objective? Objective { get; set; }

        public ICollection<UserObjectiveProgress>? UserObjectiveProgresses { get; set; }
    }
}
