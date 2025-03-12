using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class WeeklyChallenge
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = "Weekly Challenge";
        public string Description { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? RequiredPoints { get; set; } 
        public int? RequiredJournalEntries { get; set; } 
        public int? RequiredQuizzes { get; set; } 
        public int? RequiredResources { get; set; }
        public string? URLBackground { get; set; }
        public ICollection<ApplicationUserWeeklyChallenge>? Users { get; set; }

    }

}
