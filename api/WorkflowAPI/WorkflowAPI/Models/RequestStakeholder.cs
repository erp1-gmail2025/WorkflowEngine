namespace WorkflowAPI.Models
{
    public class RequestStakeholder
    {
        public int RequestID { get; set; }
        public int UserID { get; set; }

        public Request Request { get; set; }
        public User User { get; set; }
    }
}
