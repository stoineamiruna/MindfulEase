namespace MindfulEase.Models
{
    public class Badge
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; } 
        public string ImageUrl { get; set; }

        public string Activity {  get; set; }
        public ICollection<UserBadge>? Users { get; set; }
    }
}
