using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class RequestAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestActionID { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }
        public bool IsFinal { get; set; }
        public int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public Request Request { get; set; }
        public int? ActionID { get; set; }
        [ForeignKey(nameof(ActionID))]
        public WorkflowAction? Action { get; set; }
        public int TransitionID { get; set; }
        [ForeignKey(nameof(TransitionID))]
        public Transition Transition { get; set; }
    }
}