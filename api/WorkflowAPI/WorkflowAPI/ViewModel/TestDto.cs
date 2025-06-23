
namespace WorkflowAPI.ViewModel
{
    public class CreateProcessDto
    {
        public int AdminId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
    }

    public class AddActionTargetDto
    {
        public int TargetId { get; set; }
        public int GroupId { get; set; }
    }

    public class AddActivityTargetDto
    {
        public int TargetId { get; set; }
        public int GroupId { get; set; }
    }

    public class AddCustomEntityDto
    {
        public string EntityType { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

    public class AddRequestStateHolderDto
    {
        public int UserId { get; set; }
    }

    //
    public class AddStateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? StateTypeId { get; set; }
    }

    public class AddTransitionDto
    {
        public string CurrentStateName { get; set; }
        public string NextStateName { get; set; }
        public bool IsFinal { get; set; }
    }

    public class CreateActionDto
    {
        public int ActionTypeId { get; set; }
        public int ProcessId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TargetId { get; set; }
        public int GroupId { get; set; }
    }

    public class AddTransitionActionDto
    {
        public int ActionId { get; set; }
    }

    public class AddActivityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ActivityTypeId { get; set; }
        public int TargetId { get; set; }
        public int GroupId { get; set; }
    }

    public class AddTransitionActivityDto
    {
        public int ActivityId { get; set; }
    }

    public class CreateGroupDto
    {
        public string Name { get; set; }
    }

    public class AddGroupMemberDto
    {
        public int UserId { get; set; }
    }

    public class CreateRequestDto
    {
        public int ProcessId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string? InitialStateName { get; set; }
    }

    public class PerformActionDto
    {
        public int TransitionId { get; set; }
        public int UserId { get; set; }
        public string? FailureReason { get; set; }
    }

    public class AddRequestNoteDto
    {
        public int UserId { get; set; }
        public string Note { get; set; }
    }

    public class AddRequestFileDto
    {
        public int UserId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string MimeType { get; set; }
    }

    public class AddRequestStakeholderDto
    {
        public int UserId { get; set; }
    }

    public class PerformActionByStateIdDto
{
    public int NextStateId { get; set; }
    public int UserId { get; set; }
    public string? FailureReason { get; set; }
}
}