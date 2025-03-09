namespace MindfulEase.Models
{
    public class Objective
    {
        public int Id { get; set; }
        public string Title { get; set; }  
        public string Category { get; set; }  
        public string ValueType { get; set; }

        public ICollection<UserObjective>? Users { get; set; }
    }
}
