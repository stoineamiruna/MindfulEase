using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string ResourceType { get; set; } // Podcast, articol, ghid etc.
        public virtual ICollection<ApplicationUserResource>? Users { get; set; }
    }

}
