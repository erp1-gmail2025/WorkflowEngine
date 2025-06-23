using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowAPI.Models
{
    public class RequestFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestFileID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string MIMETYPE { get; set; }

        public int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public Request Request { get; set; }
        public int UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public User User { get; set; }
    }
}