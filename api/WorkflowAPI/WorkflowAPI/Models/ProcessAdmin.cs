using WorkflowAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace WorkflowAPI.Models
{
    public class ProcessAdmin
    {
        public int ProcessID { get; set; }
        public int UserID { get; set; }
        
        public Process Process { get; set; }
        public User User { get; set; }
    }
}
