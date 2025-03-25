namespace MindfulEase.Models
{
    public class ResourceTag
    {
        public int? TagId { get; set; }
        public virtual Tag? Tag { get; set; }
        public int? ResourceId { get; set; }
        public virtual Resource? Resource { get; set; }
    }
}
