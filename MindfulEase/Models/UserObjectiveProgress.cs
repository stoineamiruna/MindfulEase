using System.ComponentModel.DataAnnotations;
namespace MindfulEase.Models

{
    public class UserObjectiveProgress
    {
        [Key]
        public int Id { get; set; }
        public int? UserObjectiveId { get; set; }  
        public DateTime Date { get; set; }  
        public bool IsCompleted { get; set; }

        public UserObjective? UserObjective { get; set; }
    }
}
