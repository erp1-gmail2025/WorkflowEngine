using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class ActionType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionTypeID { get; set; }
        public string Name { get; set; }

        public List<WorkflowAction> Actions { get; set; }
    }
}