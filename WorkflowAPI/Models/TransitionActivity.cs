namespace WorkflowAPI.Models
{
    public class TransitionActivity
    {
        public int TransitionID { get; set; }
        public int ActivityID { get; set; }

        public Transition Transition { get; set; }
        public Activity Activity { get; set; }
    }
}