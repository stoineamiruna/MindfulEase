namespace MindfulEase.Models
{
    public class ApplicationUserQuiz
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
