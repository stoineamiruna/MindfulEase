namespace MindfulEase.Models
{
    public class QuizTag
    {
        public int? TagId { get; set; }
        public virtual Tag? Tag { get; set; }
        public int? QuizId { get; set; }
        public virtual Quiz? Quiz { get; set; }
    }
}
