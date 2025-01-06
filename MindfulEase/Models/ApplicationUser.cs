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
        public int? ReputationPoints { get; set; }
        public string? EmailAddress { get; set; }

        // [Required(ErrorMessage = "Data nașterii este obligatorie.")]
        public DateTime? Birthday { get; set; }

        //[Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        // Relații One-to-Many
        public ICollection<Diary> Diaries { get; set; }
        public ICollection<WeeklyReport> WeeklyReports { get; set; }
        public ICollection<Routine> Routines { get; set; }
        public ICollection<Statistics> Statistics { get; set; }
        public ICollection<Reward> Rewards { get; set; }

        // Relație Many-to-Many
        public ICollection<ApplicationUserTherapeuticGame> TherapeuticGames { get; set; }
        public ICollection<ApplicationUserEmotion> Emotions { get; set; }
        public ICollection<ApplicationUserQuiz> Quizzes { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
