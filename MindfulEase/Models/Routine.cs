using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Routine
    {
        [Key]
        public int Id { get; set; }
        public int ClusterId { get; set; }
        public string RoutineDescription { get; set; }
        public virtual ICollection<Resource>? Resources { get; set; }
    }

}
