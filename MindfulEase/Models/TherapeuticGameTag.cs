namespace MindfulEase.Models
{
    public class TherapeuticGameTag
    {
        public int? TagId { get; set; }
        public virtual Tag? Tag { get; set; }
        public int? TherapeuticGameId { get; set; }
        public virtual TherapeuticGame? TherapeuticGame { get; set; }
    }
}
