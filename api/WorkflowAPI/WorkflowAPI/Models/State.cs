using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class State
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateID { get; set; }
        public int ProcessID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StateTypeID { get; set; }
        public bool IsFinal { get; set; }
        public int StateOrder { get; set; }
        public StateType StateType { get; set; }
        public List<Request> Requests { get; set; } = [];
        public List<Transition> TransitionsFrom { get; set; } = [];
        public List<Transition> TransitionsTo { get; set; } = [];
    }
}