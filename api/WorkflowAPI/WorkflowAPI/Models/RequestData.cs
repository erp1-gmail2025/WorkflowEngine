using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowAPI.Models
{
    public class RequestData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestDataID { get; set; }
        public int RequestID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public Request Request { get; set; }
    }
}