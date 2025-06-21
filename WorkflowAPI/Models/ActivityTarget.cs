using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class ActivityTarget
    {
        public int ActivityID { get; set; }
        public int TargetID { get; set; }
        public int GroupID { get; set; }

        public Activity Activity { get; set; }
        public Target Target { get; set; }
        public Group Group { get; set; }
    }
}