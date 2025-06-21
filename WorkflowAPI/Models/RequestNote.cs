using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowAPI.Models
{
    public class RequestNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestNoteID { get; set; }
        public int RequestID { get; set; }
        public int UserID { get; set; }
        public string Note { get; set; }

        public Request Request { get; set; }
        public User User { get; set; }
    }
}