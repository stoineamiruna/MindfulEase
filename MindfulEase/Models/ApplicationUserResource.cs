namespace MindfulEase.Models
{
    public class ApplicationUserResource
    {
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? ResourceId { get; set; }
        public virtual Resource? Resource { get; set; }

        public bool? IsLiked { get; set; }
    }
}
