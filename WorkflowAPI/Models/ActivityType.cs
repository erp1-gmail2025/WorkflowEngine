using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class ActivityType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityTypeID { get; set; }
        public string Name { get; set; }

        public List<Activity> Activities { get; set; }
    }
}