namespace MindfulEase.Models
{
    public class UserObjectiveProgress
    {
        public int Id { get; set; }
        public int UserObjectiveId { get; set; }  
        public DateTime Date { get; set; }  
        public bool IsCompleted { get; set; }

        public UserObjective? UserObjective { get; set; }
    }
}
