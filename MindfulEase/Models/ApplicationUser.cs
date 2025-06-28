using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Routing;

namespace MindfulEase.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100, ErrorMessage = "First name cannot have more than 100 characters")]
        public string? FirstName { get; set; }

        [StringLength(100, ErrorMessage = "Last name cannot have more than 100 characters")]
        public string? LastName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? EmailAddress { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        [MaxLength(500, ErrorMessage = "Description too long.")]
        public string? Description { get; set; }
        [Range(8, 130, ErrorMessage = "Age must be between 0 and 130.")]
        public int? Age { get; set; }
        public string? Sex { get; set; }

        // Properties for therapist-specific information
        [Required(ErrorMessage = "Please specify if the user is a therapist.")]
        public bool? IsTherapist { get; set; } 

        [Url(ErrorMessage = "Please provide a valid URL for the profile picture.")]
        public string? ProfilePicture { get; set; } 

        [StringLength(200, ErrorMessage = "Studies information cannot exceed 200 characters.")]
        public string? Studies { get; set; } 

        [Phone(ErrorMessage = "Please enter a valid phone number.")]

        public string? BackgroundColor { get; set; } 

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public double? Rating { get; set; } 

        [Range(0, int.MaxValue, ErrorMessage = "Reviews count cannot be negative.")]
        public int? NumberOfReviews { get; set; } 

        public int? ClusterId { get; set; }

        public ICollection<Diary>? Diaries { get; set; }
        public ICollection<WeeklyReport>? WeeklyReports { get; set; }
        public ICollection<Reward>? Rewards { get; set; }
        public ICollection<ApplicationUserTherapeuticGame>? TherapeuticGames { get; set; }
        public ICollection<ApplicationUserQuiz>? Quizzes { get; set; }
        public ICollection<ApplicationUserQuestionQuiz>? UserQuestionQuizzes { get; set; }
        public ICollection<ApplicationUserResource>? Resources { get; set; }
        public ICollection<UserObjective>? Objectives { get; set; }
        public ICollection<UserBadge>? Badges { get; set; }
        public ICollection<ApplicationUserWeeklyChallenge> WeeklyChallenges { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<ApplicationUserEmotion>? Emotions { get; set; }
        public ICollection<SavedResource>? SavedResources { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
       

    }
}
