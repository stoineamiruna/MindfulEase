using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Objective
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        [MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; }
        [Required(ErrorMessage = "ValueType is required.")]
        [MaxLength(50, ErrorMessage = "ValueType cannot exceed 50 characters.")]
        public string ValueType { get; set; }
        public ICollection<UserObjective>? Users { get; set; }
    }
}
