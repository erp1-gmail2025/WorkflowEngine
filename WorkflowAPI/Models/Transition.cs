using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class Transition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransitionID { get; set; }
        public int ProcessID { get; set; }
        public int CurrentStateID { get; set; }
        public int NextStateID { get; set; }
        public bool IsFinal { get; set; }
        public State CurrentState { get; set; }
        public State NextState { get; set; }
        public List<TransitionAction> TransitionActions { get; set; }
        public List<TransitionActivity> TransitionActivities { get; set; }
        public List<RequestAction> RequestActions { get; set; }
        public Process Process { get; set; }
    }
}