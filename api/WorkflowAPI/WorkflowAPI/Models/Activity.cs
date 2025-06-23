using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityID { get; set; }
        public int ActivityTypeID { get; set; }
        public int ProcessID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Process Process { get; set; }
        public ActivityType ActivityType { get; set; }
        public List<TransitionActivity> TransitionActivities { get; set; }
        public List<ActivityTarget> ActivityTargets { get; set; }
    }
}