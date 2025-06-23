using System.Text;
using Microsoft.EntityFrameworkCore;
using WorkflowAPI.Data;
using WorkflowAPI.Models;

namespace WorkflowAPI.Service
{
    public class WorkflowEngine
    {
        private readonly ApplicationDbContext _context;

        public WorkflowEngine(ApplicationDbContext context)
        {
            _context = context;
        }

        // Khởi tạo dữ liệu cơ bản
        public async Task InitializeData()
        {
            if (!await _context.Users.AnyAsync())
            {
                var users = new List<User>
                {

                    new() {
                        FirstName = "Admin",
                        LastName = "User",
                        DateOfBirth = DateTime.UtcNow
                    },
                    new() {
                        FirstName = "Test",
                        LastName = "User",
                        DateOfBirth = DateTime.UtcNow
                    }
                };
                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
            }
            if (!await _context.StateTypes.AnyAsync())
                {
                    var stateTypes = new List<StateType>
                    {
                        new StateType { Name = "Failed", Key = "Failed", States = [] },
                        new StateType { Name = "Completed", Key = "Completed", States = [] }
                    };
                    _context.StateTypes.AddRange(stateTypes);
                }

            if (!await _context.ActionTypes.AnyAsync())
            {
                var actionTypes = new List<ActionType>
            {
                new() { Name = "Approval", Actions = [] },
                new() { Name = "Rejection", Actions = [] }
            };
                _context.ActionTypes.AddRange(actionTypes);
            }

            if (!await _context.ActivityTypes.AnyAsync())
            {
                var activityTypes = new List<ActivityType>
            {
                new() { Name = "Notification", Activities = [] },
                new() { Name = "Review", Activities = [] }
            };
                _context.ActivityTypes.AddRange(activityTypes);
            }

            if (!await _context.Targets.AnyAsync())
            {
                var targets = new List<Target>
            {
                new() { Name = "Manager", Description = "Manager role", ActionTargets = [], ActivityTargets = [] },
                new() { Name = "HR", Description = "HR role", ActionTargets = [], ActivityTargets = [] }
            };
                _context.Targets.AddRange(targets);
            }

            await _context.SaveChangesAsync();
        }

        // Tạo quy trình mới
        public async Task<Process> CreateProcess(int adminId, string name)
        {
            var admin = await _context.Users.FindAsync(adminId)
                ?? throw new Exception("Admin not found");

            var process = new Process
            {
                Name = name,
                Admins = [new ProcessAdmin { UserID = adminId, User = admin }],
                States = [],
                Transitions = [],
                Activities = [],
                Actions = []
            };
            _context.Processes.Add(process);
            await _context.SaveChangesAsync();

            var failedStateType = await EnsureStateType("Failed");
            var completedStateType = await EnsureStateType("Completed");

            var failedState = new State
            {
                ProcessID = process.ProcessID,
                Name = "Failed",
                Description = "Process failed",
                StateTypeID = failedStateType.StateTypeID,
                IsFinal = true,
                StateOrder = 0,
                TransitionsFrom = [],
                TransitionsTo = []
            };

            var completedState = new State
            {
                ProcessID = process.ProcessID,
                Name = "Completed",
                Description = "Process completed",
                StateTypeID = completedStateType.StateTypeID,
                IsFinal = true,
                StateOrder = 0,
                TransitionsFrom = [],
                TransitionsTo = []
            };

            _context.States.AddRange(failedState, completedState);
            process.States.Add(failedState);
            process.States.Add(completedState);
            await _context.SaveChangesAsync();

            return process;
        }

        public async Task<List<object>> GetProcesses()
        {
            var processes = await _context.Processes
                .Select(p => new
                {
                    p.ProcessID,
                    p.Name
                })
                .ToListAsync();
            return [.. processes.Cast<object>()];
        }
        private async Task<StateType> EnsureStateType(string name)
        {
            var stateType = await _context.StateTypes.FirstOrDefaultAsync(st => st.Key == name);
            if (stateType == null)
            {
                stateType = new StateType { Name = name, Key = name, States = [] };
                _context.StateTypes.Add(stateType);
                await _context.SaveChangesAsync();
            }
            return stateType;
        }

        public async Task<State> AddState(int processId, string name, string description, int? stateTypeId = null, int? actionId = null, int? activityId = null)
        {
            var process = await _context.Processes
                .Include(p => p.States)
                .Include(p => p.Transitions).AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProcessID == processId)
                ?? throw new Exception("Process not found");

            if (process.States.Any(s => s.Name.Normalize(NormalizationForm.FormD) == name.Normalize(NormalizationForm.FormD) && s.Description == description))
                throw new Exception("State with the same name and description already exists");

            var stateType = stateTypeId.HasValue
                ? await _context.StateTypes.FindAsync(stateTypeId.Value) ?? throw new Exception("StateType not found")
                : await EnsureStateType(name);

            var state = new State
            {
                ProcessID = processId,
                Name = name,
                Description = description,
                StateTypeID = stateType.StateTypeID,
                IsFinal = false,
                StateOrder = process.States.Count != 0 ? process.States.Max(s => s.StateOrder) + 1 : 1,
                TransitionsFrom = [],
                TransitionsTo = []
            };

            _context.States.Add(state);
            process.States.Add(state);
            await _context.SaveChangesAsync();


            var prevState = process.States
                .Where(s => !s.IsFinal && s.StateID != state.StateID)
                .OrderByDescending(s => s.StateOrder)
                .FirstOrDefault();

            var completedState = process.States.FirstOrDefault(s => s.Name == "Completed")
                ?? throw new Exception("Completed state not found");
            var failedState = process.States.FirstOrDefault(s => s.Name == "Failed")
                ?? throw new Exception("Failed state not found");

            var oldTransitionToCompleted = process.Transitions
                .FirstOrDefault(t => t.NextStateID == completedState.StateID && t.CurrentStateID != state.StateID);
            if (oldTransitionToCompleted != null)
            {
                var oldCurrentState = process.States.FirstOrDefault(s => s.StateID == oldTransitionToCompleted.CurrentStateID);
                oldCurrentState?.TransitionsTo?.Remove(oldTransitionToCompleted);
                completedState.TransitionsFrom?.Remove(oldTransitionToCompleted);
                process.Transitions.Remove(oldTransitionToCompleted);
                _context.Transitions.Remove(oldTransitionToCompleted);
                await _context.SaveChangesAsync();
            }

            Transition? transitionToNewState = null;
            if (prevState != null)
            {
                transitionToNewState = await AddTransition(processId, prevState.Name, state.Name, isFinal: false);
            }

            var transitionToCompleted = await AddTransition(processId, state.Name, completedState.Name, isFinal: true);

            var transitionToFailed = await AddTransition(processId, state.Name, failedState.Name, isFinal: true);

            if (actionId.HasValue)
            {
                await AddTransitionAction(transitionToCompleted.TransitionID, actionId.Value);
            }
            if (activityId.HasValue)
            {
                await AddTransitionActivity(transitionToCompleted.TransitionID, activityId.Value);
            }

            return state;
        }

        public async Task<Transition> AddTransition(int processId, string currentStateName, string nextStateName, bool isFinal = false)
        {
            var process = await _context.Processes
                .Include(p => p.States)
                .Include(p => p.Transitions).AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProcessID == processId)
                ?? throw new Exception("Process not found");

            var sortedStates = process.States.OrderByDescending(s => s.StateOrder).ToList();

            var currentState = sortedStates.FirstOrDefault(s => s.Name.Normalize(NormalizationForm.FormD) == currentStateName.Normalize(NormalizationForm.FormD))
                ?? throw new Exception("Current state not found");
            var nextState = sortedStates.FirstOrDefault(s => s.Name.Normalize(NormalizationForm.FormD) == nextStateName.Normalize(NormalizationForm.FormD))
                ?? throw new Exception("Next state not found");

            if (currentState.StateID == nextState.StateID)
            {
                currentState = sortedStates
                    .Where(s => !s.IsFinal && s.StateID != nextState.StateID)
                    .FirstOrDefault() ?? throw new Exception("No valid previous state found to replace current state");
            }

            currentState.TransitionsTo ??= [];
            nextState.TransitionsFrom ??= [];
            process.Transitions ??= [];

            var transition = new Transition
            {
                ProcessID = processId,
                CurrentStateID = currentState.StateID,
                NextStateID = nextState.StateID,
                IsFinal = isFinal,
                TransitionActions = [],
                TransitionActivities = [],
            };

            _context.Transitions.Add(transition);
            await _context.SaveChangesAsync();
            currentState.TransitionsTo.Add(transition);
            nextState.TransitionsFrom.Add(transition);
            process.Transitions.Add(transition);
            return transition;
        }

        // Tạo hành động
        public async Task<WorkflowAction> CreateAction(int actionTypeId, int processId, string name, string description, int targetId, int groupId)
        {
            var actionType = await _context.ActionTypes.FindAsync(actionTypeId)
                ?? throw new Exception("ActionType not found");
            var process = await _context.Processes.FindAsync(processId)
                ?? throw new Exception("Process not found");
            var target = await _context.Targets.FindAsync(targetId)
                ?? throw new Exception("Target not found");
            var group = await _context.Groups.FindAsync(groupId)
                ?? throw new Exception("Group not found");

            var action = new WorkflowAction
            {
                ActionTypeID = actionTypeId,
                ProcessID = processId,
                Name = name,
                Description = description,
                ActionType = actionType,
                Process = process,
                RequestActions = [],
                TransitionActions = [],
                ActionTargets = [new ActionTarget { TargetID = targetId, GroupID = groupId, Target = target, Group = group }]
            };

            _context.WorkflowActions.Add(action);
            actionType.Actions.Add(action);
            process.Actions.Add(action);
            target.ActionTargets.Add(action.ActionTargets.First());
            group.ActionTargets.Add(action.ActionTargets.First());
            await _context.SaveChangesAsync();

            return action;
        }

        public async Task AddTransitionAction(int transitionId, int actionId)
        {
            var transition = await _context.Transitions
                .Include(t => t.TransitionActions)
                .FirstOrDefaultAsync(t => t.TransitionID == transitionId)
                ?? throw new Exception("Transition not found");

            var action = await _context.WorkflowActions.FindAsync(actionId)
                ?? throw new Exception("Action not found");

            var transitionAction = new TransitionAction
            {
                TransitionID = transitionId,
                ActionID = actionId,
                Transition = transition,
                Action = action
            };

            _context.TransitionActions.Add(transitionAction);
            transition.TransitionActions.Add(transitionAction);
            await _context.SaveChangesAsync();
        }

        // Tạo hoạt động
        public async Task<Activity> AddActivity(int processId, string name, string description, int activityTypeId, int targetId, int groupId)
        {
            var process = await _context.Processes.FindAsync(processId)
                ?? throw new Exception("Process not found");
            var activityType = await _context.ActivityTypes.FindAsync(activityTypeId)
                ?? throw new Exception("ActivityType not found");
            var target = await _context.Targets.FindAsync(targetId)
                ?? throw new Exception("Target not found");
            var group = await _context.Groups.FindAsync(groupId)
                ?? throw new Exception("Group not found");

            var activity = new Activity
            {
                ProcessID = processId,
                Name = name,
                Description = description,
                ActivityTypeID = activityTypeId,
                ActivityType = activityType,
                Process = process,
                TransitionActivities = [],
                ActivityTargets = [new ActivityTarget { TargetID = targetId, GroupID = groupId, Target = target, Group = group }]
            };

            _context.Activities.Add(activity);
            process.Activities.Add(activity);
            activityType.Activities.Add(activity);
            target.ActivityTargets.Add(activity.ActivityTargets.First());
            group.ActivityTargets.Add(activity.ActivityTargets.First());
            await _context.SaveChangesAsync();

            return activity;
        }

        // Gắn hoạt động vào chuyển đổi
        public async Task AddTransitionActivity(int transitionId, int activityId)
        {
            var transition = await _context.Transitions
                .Include(t => t.TransitionActivities)
                .FirstOrDefaultAsync(t => t.TransitionID == transitionId)
                ?? throw new Exception("Transition not found");

            var activity = await _context.Activities.FindAsync(activityId)
                ?? throw new Exception("Activity not found");

            var transitionActivity = new TransitionActivity
            {
                TransitionID = transitionId,
                ActivityID = activityId,
                Transition = transition,
                Activity = activity
            };

            _context.TransitionActivities.Add(transitionActivity);
            transition.TransitionActivities.Add(transitionActivity);
            await _context.SaveChangesAsync();
        }

        // Tạo nhóm
        public async Task<Group> CreateGroup(int processId, string name)
        {
            var process = await _context.Processes.FindAsync(processId)
                ?? throw new Exception("Process not found");

            var group = new Group
            {
                Name = name,
                GroupMembers = [],
                ActionTargets = [],
                ActivityTargets = []
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        // Thêm thành viên vào nhóm
        public async Task AddGroupMember(int groupId, int userId)
        {
            var group = await _context.Groups
                .Include(g => g.GroupMembers)
                .FirstOrDefaultAsync(g => g.GroupID == groupId)
                ?? throw new Exception("Group not found");

            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("User not found");

            var groupMember = new GroupMember
            {
                GroupID = groupId,
                UserID = userId,
                Group = group,
                User = user
            };

            _context.GroupMembers.Add(groupMember);
            group.GroupMembers.Add(groupMember);
            user.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();
        }

        // Tạo request
        public async Task<Request> CreateRequest(int processId, int userId, string title, Dictionary<string, string> data, string? initialStateName)
        {
            var process = await _context.Processes
                .Include(p => p.States)
                .ThenInclude(s => s.StateType)
                .FirstOrDefaultAsync(p => p.ProcessID == processId)
                ?? throw new Exception("Process not found");

            State initialState;
            if (!string.IsNullOrEmpty(initialStateName))
            {
                initialState = process.States.FirstOrDefault(s => s.Name.Normalize(NormalizationForm.FormD) == initialStateName.Normalize(NormalizationForm.FormD))
                    ?? throw new Exception($"Initial state '{initialStateName}' not found in process");
                if (initialState.IsFinal)
                    throw new Exception("Initial state cannot be a final state");
            }
            else
            {
                initialState = process.States
                    .Where(s => !s.IsFinal)
                    .OrderBy(s => s.StateOrder)
                    .FirstOrDefault()
                    ?? throw new Exception("No valid initial state found in process");
            }

            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("User not found");

            var request = new Request
            {
                ProcessID = processId,
                Title = title,
                DateRequested = DateTime.UtcNow,
                UserID = userId,
                CurrentStateID = initialState.StateID,
                User = user,
                CurrentState = initialState,
                RequestNotes = [],
                RequestData = [],
                RequestFiles = [],
                RequestStakeholders = [],
                RequestActions = []
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
            var requestDataToAdd = new List<RequestData>();
            foreach (var item in data)
            {
                requestDataToAdd.Add(new RequestData
                {
                    RequestID = request.RequestID,
                    Name = item.Key,
                    Value = item.Value,
                    Request = request
                });
            }
            if (requestDataToAdd.Count > 0)
            {
                _context.RequestData.AddRange(requestDataToAdd);
                await _context.SaveChangesAsync();
            }
            (user.Requests ?? []).Add(request);
            (process.Requests ?? []).Add(request);

            return request;
        }

        // Thực hiện hành động trên request
        public async Task PerformAction(int requestId, int transitionId, int userId, string? failureReason = null)
        {
            var request = await _context.Requests
                .Include(r => r.CurrentState)
                .Include(r => r.RequestActions)
                .Include(r => r.RequestData)
                .FirstOrDefaultAsync(r => r.RequestID == requestId)
                ?? throw new Exception("Request not found");

            var transition = await _context.Transitions
                .Include(t => t.NextState)
                .Include(t => t.TransitionActions)
                .ThenInclude(ta => ta.Action)
                .ThenInclude(a => a.ActionTargets)
                .ThenInclude(at => at.Group)
                .ThenInclude(g => g.GroupMembers)
                .Include(t => t.TransitionActivities)
                .ThenInclude(ta => ta.Activity)
                .FirstOrDefaultAsync(t => t.TransitionID == transitionId && t.CurrentStateID == request.CurrentStateID)
                ?? throw new Exception("Invalid transition for current state");

            var action = transition.TransitionActions.FirstOrDefault()?.Action;
            if (action != null)
            {
                var authorized = action.ActionTargets.Any(at => at.Group.GroupMembers.Any(gm => gm.UserID == userId));
                if (!authorized)
                    throw new Exception("User is not authorized to perform this action");
            }

            if (transition.NextState.Name == "Failed" && string.IsNullOrWhiteSpace(failureReason))
                throw new Exception("Failure reason is required when transitioning to Failed state");

            var nextState = transition.NextState;
            if (nextState.IsFinal && nextState.Name == "Completed" && !request.RequestActions.All(a => a.IsComplete))
                throw new Exception("Cannot complete until all steps are approved");

            request.CurrentStateID = nextState.StateID;
            request.CurrentState = nextState;

            if (transition.NextState.Name == "Failed" && !string.IsNullOrWhiteSpace(failureReason))
            {
                request.RequestData.Add(new RequestData
                {
                    RequestID = requestId,
                    Name = "FailureReason",
                    Value = failureReason,
                    Request = request
                });
            }

            var requestAction = new RequestAction
            {
                RequestID = requestId,
                TransitionID = transitionId,
                ActionID = action?.WorkflowActionID ?? 0,
                IsActive = true,
                IsComplete = nextState.IsFinal,
                IsFinal = transition.IsFinal,
                Request = request,
                Transition = transition
            };

            _context.RequestActions.Add(requestAction);
            request.RequestActions.Add(requestAction);
            _context.Requests.Update(request);

            foreach (var ta in transition.TransitionActions)
            {
                Console.WriteLine($"Executing Action: {ta.Action.Name}");
            }
            foreach (var ta in transition.TransitionActivities)
            {
                Console.WriteLine($"Executing Activity: {ta.Activity.Name}");
            }

            await _context.SaveChangesAsync();
        }

        public async Task PerformActionByNextStateId(int requestId, int nextStateId, int userId, string? failureReason = null)
        {
            var request = await _context.Requests
                .Include(r => r.CurrentState)
                .Include(r => r.RequestActions)
                .Include(r => r.RequestData)
                .FirstOrDefaultAsync(r => r.RequestID == requestId)
                ?? throw new Exception("Request not found");

            var transition = await _context.Transitions
                .Include(t => t.NextState)
                .Include(t => t.TransitionActions)
                .ThenInclude(ta => ta.Action)
                .ThenInclude(a => a.ActionTargets)
                .ThenInclude(at => at.Group)
                .ThenInclude(g => g.GroupMembers)
                .Include(t => t.TransitionActivities)
                .ThenInclude(ta => ta.Activity)
                .FirstOrDefaultAsync(t => t.CurrentStateID == request.CurrentStateID && t.NextStateID == nextStateId)
                ?? throw new Exception($"No valid transition found from state '{request.CurrentState.Name}' to state with ID '{nextStateId}'");

            await PerformTransition(request, transition, userId, failureReason);
        }

        private async Task PerformTransition(Request request, Transition transition, int userId, string? failureReason)
        {
            var action = transition.TransitionActions.FirstOrDefault()?.Action;
            if (action != null)
            {
                var authorized = action.ActionTargets.Any(at => at.Group.GroupMembers.Any(gm => gm.UserID == userId));
                if (!authorized)
                    throw new Exception("User is not authorized to perform this action");
            }

            if (transition.NextState.Name == "Failed" && (string.IsNullOrWhiteSpace(failureReason) || failureReason.Length < 10))
                throw new Exception("Failure reason is required and must be at least 10 characters long when transitioning to Failed state");

            var nextState = transition.NextState;
            if (nextState.IsFinal && nextState.Name == "Completed" && !request.RequestActions.All(a => a.IsComplete))
                throw new Exception("Cannot complete until all steps are approved");

            request.CurrentStateID = nextState.StateID;
            request.CurrentState = nextState;

            if (transition.NextState.Name == "Failed" && !string.IsNullOrWhiteSpace(failureReason))
            {
                request.RequestData.Add(new RequestData
                {
                    RequestID = request.RequestID,
                    Name = "FailureReason",
                    Value = failureReason,
                    Request = request
                });
            }

            var requestAction = new RequestAction
            {
                RequestID = request.RequestID,
                TransitionID = transition.TransitionID,
                ActionID = action?.WorkflowActionID,
                IsActive = true,
                IsComplete = nextState.IsFinal,
                IsFinal = transition.IsFinal,
                Request = request,
                Transition = transition
            };

            _context.RequestActions.Add(requestAction);
            request.RequestActions.Add(requestAction);
            _context.Requests.Update(request);

            (request.RequestNotes ?? []).Add(new RequestNote
            {
                RequestID = request.RequestID,
                UserID = userId,
                Note = $"Transition from {request.CurrentState.Name} to {nextState.Name}{(nextState.Name == "Failed" ? $": {failureReason}" : "")}",
                // CreatedDate = DateTime.UtcNow,
                Request = request
            });

            foreach (var ta in transition.TransitionActions)
            {
                Console.WriteLine($"Executing Action: {ta.Action.Name}");
            }
            foreach (var ta in transition.TransitionActivities)
            {
                Console.WriteLine($"Executing Activity: {ta.Activity.Name}");
            }

            await _context.SaveChangesAsync();
        }

        // Thêm ghi chú cho request
        public async Task AddRequestNote(int requestId, int userId, string note)
        {
            var request = await _context.Requests
                .Include(r => r.RequestNotes)
                .FirstOrDefaultAsync(r => r.RequestID == requestId)
                ?? throw new Exception("Request not found");

            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("User not found");

            var requestNote = new RequestNote
            {
                RequestID = requestId,
                UserID = userId,
                Note = note,
                Request = request,
                User = user
            };

            _context.RequestNotes.Add(requestNote);
            request.RequestNotes.Add(requestNote);
            user.RequestNotes.Add(requestNote);
            await _context.SaveChangesAsync();
        }

        public async Task<List<State>> GetAvailableNextStates(int requestId)
        {
            var request = await _context.Requests
                .Include(r => r.CurrentState)
                .FirstOrDefaultAsync(r => r.RequestID == requestId)
                ?? throw new Exception("Request not found");

            var nextStates = await _context.Transitions
                .Include(t => t.NextState)
                .Where(t => t.CurrentStateID == request.CurrentStateID)
                .Select(t => t.NextState)
                .Distinct()
                .OrderBy(s => s.StateOrder)
                .ToListAsync();

            return nextStates;
        }

        // Thêm tệp cho request
        public async Task AddRequestFile(int requestId, int userId, string fileName, byte[] fileContent, string mimeType)
        {
            var request = await _context.Requests
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(r => r.RequestID == requestId)
                ?? throw new Exception("Request not found");

            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("User not found");

            var requestFile = new RequestFile
            {
                RequestID = requestId,
                UserID = userId,
                FileName = fileName,
                FileContent = fileContent,
                MIMETYPE = mimeType,
                Request = request,
                User = user
            };

            _context.RequestFiles.Add(requestFile);
            request.RequestFiles.Add(requestFile);
            user.RequestFiles.Add(requestFile);
            await _context.SaveChangesAsync();
        }

        // Thêm người liên quan cho request
        public async Task AddRequestStakeholder(int requestId, int userId)
        {
            var request = await _context.Requests
                .Include(r => r.RequestStakeholders)
                .FirstOrDefaultAsync(r => r.RequestID == requestId)
                ?? throw new Exception("Request not found");

            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("User not found");

            var requestStakeholder = new RequestStakeholder
            {
                RequestID = requestId,
                UserID = userId,
                Request = request,
                User = user
            };

            _context.RequestStakeholders.Add(requestStakeholder);
            request.RequestStakeholders.Add(requestStakeholder);
            user.RequestStakeholders.Add(requestStakeholder);
            await _context.SaveChangesAsync();
        }

        // Lấy danh sách trạng thái của quy trình
        public async Task<List<State>> GetProcessStates(int processId)
        {
            var process = await _context.Processes
                .Include(p => p.States)
                .ThenInclude(s => s.StateType)
                .FirstOrDefaultAsync(p => p.ProcessID == processId)
                ?? throw new Exception("Process not found");

            return [.. process.States];
        }

        public async Task<List<object>> GetRequestsByProcessId(int processId)
        {
            var requests = await _context.Requests
                .Where(r => r.ProcessID == processId)
                .Include(r => r.CurrentState)
                .Select(r => new
                {
                    r.RequestID,
                    r.Title,
                    r.DateRequested,
                    r.CurrentStateID,
                    CurrentStateName = r.CurrentState.Name,
                    CurrentStateDescription = r.CurrentState.Description
                })
                .ToListAsync();
            return [.. requests.Cast<object>()];
        }

        public async Task<Process?> GetTransitions(int processId)
        {
            return await _context.Processes
                .Include(p => p.Transitions)
                .ThenInclude(t => t.CurrentState)
                .Include(p => p.Transitions)
                .ThenInclude(t => t.NextState)
                .FirstOrDefaultAsync(p => p.ProcessID == processId);
        }
    }
}