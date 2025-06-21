using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupID { get; set; }
        public string Name { get; set; }

        public List<GroupMember> GroupMembers { get; set; }
        public List<ActionTarget> ActionTargets { get; set; }
        public List<ActivityTarget> ActivityTargets { get; set; }
    }
}