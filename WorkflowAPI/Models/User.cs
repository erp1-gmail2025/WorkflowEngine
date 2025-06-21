using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WorkflowAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public List<GroupMember> GroupMembers { get; set; }
        public List<ProcessAdmin> ProcessAdmins { get; set; }
        public List<Request> Requests { get; set; }
        public List<RequestNote> RequestNotes { get; set; }
        public List<RequestFile> RequestFiles { get; set; }
        public List<RequestStakeholder> RequestStakeholders { get; set; }
    }
}