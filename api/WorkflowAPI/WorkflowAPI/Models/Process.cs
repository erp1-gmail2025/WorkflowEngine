using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class Process
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProcessID { get; set; }
        public string Name { get; set; }

        public List<ProcessAdmin> Admins { get; set; }
        public List<State> States { get; set; }
        public List<Transition> Transitions { get; set; }
        public List<Activity> Activities { get; set; }
        public List<WorkflowAction> Actions { get; set; }
        public List<Request> Requests { get; set; }
    }
}