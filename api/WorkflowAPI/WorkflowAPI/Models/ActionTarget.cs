using WorkflowAPI.Models;

namespace WorkflowAPI.Models
{
    public class ActionTarget
    {
        public int ActionID { get; set; }
        public int TargetID { get; set; }
        public int GroupID { get; set; }

        public WorkflowAction Action { get; set; }
        public Target Target { get; set; }
        public Group Group { get; set; }
    }
}