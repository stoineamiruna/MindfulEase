using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class TherapeuticGame
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Instructions { get; set; }

        // Relație Many-to-Many
        public virtual ICollection<ApplicationUserTherapeuticGame> Users { get; set; }
    }
}
