using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestID { get; set; }
        public string Title { get; set; }
        public DateTime DateRequested { get; set; }
        public int UserID { get; set; }
        public int CurrentStateID { get; set; }
        public int ProcessID { get; set; }
        [ForeignKey(nameof(ProcessID))]
        public Process Process { get; set; }
        public User User { get; set; }
        public State CurrentState { get; set; }
        public List<RequestNote> RequestNotes { get; set; }
        public List<RequestData> RequestData { get; set; }
        public List<RequestFile> RequestFiles { get; set; }
        public List<RequestStakeholder> RequestStakeholders { get; set; }
        public List<RequestAction> RequestActions { get; set; }
    }
}