namespace WorkflowAPI.Models
{
    public class GroupMember
    {
        public int UserID { get; set; }
        public int GroupID { get; set; }

        public User User { get; set; }
        public Group Group { get; set; }
    }
}