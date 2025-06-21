using WorkflowAPI.Models;

namespace WorkflowAPI.Models
{
    public class TransitionAction
    {
        public int TransitionID { get; set; }
        public int ActionID { get; set; }

        public Transition Transition { get; set; }
        public WorkflowAction Action { get; set; }
    }
}