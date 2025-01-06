using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class WeeklyChallenge
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int RewardPoints { get; set; }
    }

}
