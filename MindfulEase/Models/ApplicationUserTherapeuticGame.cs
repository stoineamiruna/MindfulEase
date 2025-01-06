namespace MindfulEase.Models
{
    public class ApplicationUserTherapeuticGame
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int GameId { get; set; }
        public virtual TherapeuticGame Game { get; set; }
    }
}
