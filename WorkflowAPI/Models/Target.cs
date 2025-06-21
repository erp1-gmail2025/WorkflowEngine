using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class Target
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TargetID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<ActionTarget> ActionTargets { get; set; }
        public List<ActivityTarget> ActivityTargets { get; set; }
    }
}