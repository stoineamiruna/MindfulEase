using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Badge
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = "Title can't exceed 100 characters.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(300, ErrorMessage = "Description can't exceed 300 characters.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Image URL must be a valid URL.")]
        [MaxLength(200, ErrorMessage = "Image URL can't exceed 200 characters.")]
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Activity is required.")]
        [MaxLength(100, ErrorMessage = "Activity name can't exceed 100 characters.")]
        public string Activity {  get; set; }
        public ICollection<UserBadge>? Users { get; set; }
    }
}
