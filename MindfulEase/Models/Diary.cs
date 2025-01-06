using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models
{
    public class Diary
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string Content { get; set; } // Conținutul jurnalului

        // Relație Many-to-One cu ApplicationUser
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }

}
