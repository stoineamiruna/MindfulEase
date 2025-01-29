using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class BrainRegion
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
