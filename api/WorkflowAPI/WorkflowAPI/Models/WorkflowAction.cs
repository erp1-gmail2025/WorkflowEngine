using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowAPI.Models
{
    public class WorkflowAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkflowActionID { get; set; }
        public int ActionTypeID { get; set; }
        public int ProcessID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Process Process { get; set; }
        public ActionType ActionType { get; set; }
        public List<RequestAction> RequestActions { get; set; }
        public List<TransitionAction> TransitionActions { get; set; }
        public List<ActionTarget> ActionTargets { get; set; }
    }
}