using System.ComponentModel.DataAnnotations;
using MindfulEase.Validation;
namespace MindfulEase.Models
{
    public class WeeklyChallenge
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = "Weekly Challenge";
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "StartDate is required.")]
        [DataType(DataType.Date)]
        [DateNotInPast(ErrorMessage = "Start date cannot be in the past.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "EndDate is required.")]
        [DataType(DataType.Date)]
        [DateGreaterThan("StartDate", ErrorMessage = "EndDate must be after StartDate.")]
        public DateTime EndDate { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "RequiredPoints must be zero or a positive number.")]
        public int? RequiredPoints { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "RequiredJournalEntries must be zero or a positive number.")]
        public int? RequiredJournalEntries { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "RequiredQuizzes must be zero or a positive number.")]
        public int? RequiredQuizzes { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "RequiredResources must be zero or a positive number.")]
        public int? RequiredResources { get; set; }
        [Url(ErrorMessage = "URLBackground must be a valid URL.")]
        [StringLength(500, ErrorMessage = "URLBackground cannot exceed 500 characters.")]
        public string? URLBackground { get; set; }
        public ICollection<ApplicationUserWeeklyChallenge>? Users { get; set; }

    }

}
