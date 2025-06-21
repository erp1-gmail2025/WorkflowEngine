using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowAPI.Models
{
    public class StateType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateTypeID { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public List<State> States { get; set; }
    }
}