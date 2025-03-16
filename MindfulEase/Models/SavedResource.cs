namespace MindfulEase.Models
{
    public class SavedResource
    {
        public string? UserId {  get; set; }
        public int? ResourceId { get; set; }
        public DateTime Date { get; set; }
        public bool? IsSaved { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Resource? Resource { get; set; }
    }
}
