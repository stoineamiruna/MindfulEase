using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Routing;

namespace MindfulEase.Models
{
    public class ApplicationUser : IdentityUser
    {
        //[Required(ErrorMessage = "First name is required")]
        // [StringLength(100, ErrorMessage = "First name cannot have more than 100 characters")]
        public string? FirstName { get; set; }
        //[Required(ErrorMessage = "Last name is required")]
        //[StringLength(100, ErrorMessage = "Last name cannot have more than 100 characters")]
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }

        // [Required(ErrorMessage = "Data nașterii este obligatorie.")]
        public DateTime? Birthday { get; set; }

        //[Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        // Properties for therapist-specific information
        [Required(ErrorMessage = "Please specify if the user is a therapist.")]
        public bool? IsTherapist { get; set; } // Indicates if the user is a therapist

        [Url(ErrorMessage = "Please provide a valid URL for the profile picture.")]
        public string? ProfilePicture { get; set; } // Path or URL to profile picture

        [StringLength(200, ErrorMessage = "Studies information cannot exceed 200 characters.")]
        public string? Studies { get; set; } // Education or qualifications

        [Phone(ErrorMessage = "Please enter a valid phone number.")]

        public string? BackgroundColor { get; set; } // Preferred background color for profile

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public double? Rating { get; set; } // Rating between 1 and 5

        [Range(0, int.MaxValue, ErrorMessage = "Reviews count cannot be negative.")]
        public int? NumberOfReviews { get; set; } // Number of reviews received

        public int? ClusterId { get; set; }
        // Relații One-to-Many
        public ICollection<Diary>? Diaries { get; set; }
        public ICollection<WeeklyReport>? WeeklyReports { get; set; }
        public ICollection<Statistics>? Statistics { get; set; }
        public ICollection<Reward>? Rewards { get; set; }

        // Relație Many-to-Many
        public ICollection<ApplicationUserTherapeuticGame>? TherapeuticGames { get; set; }
        public ICollection<ApplicationUserQuiz>? Quizzes { get; set; }
        public ICollection<ApplicationUserQuestionQuiz>? UserQuestionQuizzes { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
        public ICollection<ApplicationUserResource>? Resources { get; set; }
        public ICollection<UserObjective>? Objectives { get; set; }
        public ICollection<UserBadge>? Badges { get; set; }
        public ICollection<ApplicationUserWeeklyChallenge> WeeklyChallenges { get; set;}
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<ApplicationUserEmotion>? Emotions { get; set; }

    }
}
