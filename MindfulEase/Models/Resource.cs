using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Link is required.")]
        [Url(ErrorMessage = "Link must be a valid URL.")]
        [StringLength(500, ErrorMessage = "Link cannot be longer than 500 characters.")]
        public string Link { get; set; }
        [Required(ErrorMessage = "ResourceType is required.")]
        [StringLength(50, ErrorMessage = "ResourceType cannot be longer than 50 characters.")]
        public string ResourceType { get; set; } // Podcast, articol, ghid etc.
        public virtual ICollection<ApplicationUserResource>? Users { get; set; }
        public virtual ICollection<SavedResource>? SavedResources { get; set; }
        public virtual ICollection<ResourceTag>? Tags { get; set; }
    }

}
