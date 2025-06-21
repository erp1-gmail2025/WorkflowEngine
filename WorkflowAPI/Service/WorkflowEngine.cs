using System.Text;
using Microsoft.EntityFrameworkCore;
using WorkflowAPI.Data;
using WorkflowAPI.Models;

namespace WorkflowAPI.Service
{
    // Services/WorkflowEngine.cs
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WorkflowEngine
    {
        private readonly ApplicationDbContext _context;

        public WorkflowEngine(ApplicationDbContext context)
        {
            _context = context;
        }

        // Khởi tạo dữ liệu cơ bản với tùy chỉnh
        public async Task InitializeData(Dictionary<string, object> customData = null)
        {
            if (!await _context.Users.AnyAsync())
            {
                var users = new List<User>
            {
                new User { FirstName = "Admin", LastName = "User", DateOfBirth = new DateTime(1990, 1, 1), GroupMembers = [], ProcessAdmins = [], Requests = [], RequestNotes = [], RequestFiles = [], RequestStakeholders = [] },
                new User { FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1995, 5, 15), GroupMembers = [], ProcessAdmins = [], Requests = [], RequestNotes = [], RequestFiles = [], RequestStakeholders = [] }
            };
                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
            }

            if (!await _context.ActionTypes.AnyAsync())
            {
                var actionTypes = new List<ActionType>
            {
                new ActionType { Name = "Approval", Actions = [] },
                new ActionType { Name = "Rejection", Actions = [] }
            };
                _context.ActionTypes.AddRange(actionTypes);
                await _context.SaveChangesAsync();
            }

            if (!await _context.ActivityTypes.AnyAsync())
            {
                var activityTypes = new List<ActivityType>
            {
                new ActivityType { Name = "Notification", Activities = [] },
                new ActivityType { Name = "Review", Activities = [] }
            };
                _context.ActivityTypes.AddRange(activityTypes);
                await _context.SaveChangesAsync();
            }

            if (!await _context.Targets.AnyAsync())
            {
                var targets = new List<Target>
            {
                new Target { Name = "Manager", Description = "Manager role", ActionTargets = [], ActivityTargets = [] },
                new Target { Name = "HR", Description = "HR role", ActionTargets = [], ActivityTargets = [] }
            };
                _context.Targets.AddRange(targets);
                await _context.SaveChangesAsync();
            }

            // Xử lý dữ liệu tùy chỉnh nếu có
            if (customData != null)
            {
                foreach (var item in customData)
                {
                    if (item.Key == "CustomEntities")
                    {
                        var customEntities = item.Value as List<Dictionary<string, string>>;
                        if (customEntities != null)
                        {
                            foreach (var ce in customEntities)
                            {
                                var customEntity = new CustomEntity
                                {
                                    EntityType = ce["EntityType"],
                                    Data = ce["Data"],
                                    ProcessID = ce.ContainsKey("ProcessID") ? int.Parse(ce["ProcessID"]) : (int?)null
                                };
                                _context.CustomEntities.Add(customEntity);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        public async Task<Process> CreateProcess(int adminId, int userId, string name)
        {
            // await InitializeData();

            var admin = await _context.Users.FindAsync(adminId);
            var user = await _context.Users.FindAsync(userId);
            if (admin == null || user == null) throw new Exception("Admin or User not found");

            var process = new Process
            {
                Name = name,
                Admins = [new ProcessAdmin { UserID = adminId, User = admin, Process = null }],
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
                StateOrder = 1,
                TransitionsFrom = [],
                TransitionsTo = []
            };
            _context.States.Add(failedState);

            var completedState = new State
            {
                ProcessID = process.ProcessID,
                Name = "Completed",
                Description = "Process completed",
                StateTypeID = completedStateType.StateTypeID,
                StateOrder = 0,
                IsFinal = true,
                TransitionsFrom = [],
                TransitionsTo = []
            };
            _context.States.Add(completedState);

            await _context.SaveChangesAsync();
            process.States.Add(failedState);
            process.States.Add(completedState);

            return process;
        }

        private async Task<StateType> EnsureStateType(string name)
        {
            var normalizedName = name.Normalize(NormalizationForm.FormD);
            var stateType = await _context.StateTypes.FirstOrDefaultAsync(st => st.Key == normalizedName);
            if (stateType == null)
            {
                stateType = new StateType { Name = normalizedName, States = [], Key = normalizedName };
                _context.StateTypes.Add(stateType);
                await _context.SaveChangesAsync();
            }
            return stateType;
        }

        public async Task<State> AddState(int processId, string name, string description, int? stateTypeId = null)
        {
            var process = await _context.Processes
                .Include(p => p.States)
                .FirstOrDefaultAsync(p => p.ProcessID == processId);
            if (process == null) throw new Exception("Process not found");

            if (!process.States.Any(s => s.Name == "Failed") || !process.States.Any(s => s.Name == "Completed"))
                throw new Exception("Process must have Failed and Completed states before adding more states");

            var effectiveStateTypeId = stateTypeId ?? (await EnsureStateType(name)).StateTypeID;
            var test = process.States.Max(q => q.StateOrder);
            var state = new State
            {
                ProcessID = processId,
                Name = name,
                Description = description,
                StateTypeID = effectiveStateTypeId,
                IsFinal = false,
                StateOrder = process.States.Max(q => q.StateOrder) + 1,
                TransitionsFrom = [],
                TransitionsTo = []
            };
            _context.States.Add(state);
            await _context.SaveChangesAsync();
            var prevState = process.States
                .Where(s => s.Name != "Failed" && s.Name != "Completed" && s.StateID != state.StateID)
                .OrderByDescending(s => s.StateID)
                .FirstOrDefault();
            if (prevState != null)
            {
                await AddTransition(processId, prevState.Name, state.Name);
            }
            process.States.Add(state);
            return state;
        }

        public async Task<Transition> AddTransition(int processId, string currentStateName, string nextStateName, bool isFinal = false)
        {
            var process = await _context.Processes
                .Include(p => p.States)
                .FirstOrDefaultAsync(p => p.ProcessID == processId);
            if (process == null) throw new Exception("Process not found");
            var sortState = process.States.OrderBy(q => q.StateID);
            var currentState = sortState.LastOrDefault(s => s.Name.Normalize(NormalizationForm.FormD) == currentStateName.Normalize(NormalizationForm.FormD));
            var nextState = sortState.LastOrDefault(s => s.Name.Normalize(NormalizationForm.FormD) == nextStateName.Normalize(NormalizationForm.FormD));
            if (currentState == null || nextState == null)
                throw new Exception("Current or next state not found");
            if(currentState.StateID == nextState.StateID)
            {
                currentState = sortState.Reverse().Skip(1).First();
            }    
            var transition = new Transition
            {
                ProcessID = processId,
                CurrentStateID = currentState.StateID,
                NextStateID = nextState.StateID,
                IsFinal = isFinal,
                CurrentState = currentState,
                NextState = nextState,
                TransitionActions = [],
                TransitionActivities = [],
                Process = process
            };
            _context.Transitions.Add(transition);
            await _context.SaveChangesAsync();
            (currentState.TransitionsTo ?? []).Add(transition);
            (nextState.TransitionsFrom ?? []).Add(transition);
            (process.Transitions ?? []).Add(transition);
            return transition;
        }

        public async Task<WorkflowAction> CreateAction(int actionTypeId, int processId, string name, string description)
        {
            var actionType = await _context.ActionTypes.FindAsync(actionTypeId);
            if (actionType == null) throw new Exception("ActionType not found");

            var process = await _context.Processes.FindAsync(processId);
            if (process == null) throw new Exception("Process not found");

            var action = new WorkflowAction
            {
                ActionTypeID = actionTypeId,
                ProcessID = processId,
                Name = name,
                Description = description,
                ActionType = actionType,
                RequestActions = [],
                TransitionActions = [],
                ActionTargets = [],
                Process = process
            };
            _context.WorkflowActions.Add(action);
            await _context.SaveChangesAsync();
            actionType.Actions.Add(action);
            process.Actions.Add(action);
            return action;
        }

        public async Task AddTransitionAction(int transitionId, int actionId)
        {
            var transition = await _context.Transitions
                .Include(t => t.TransitionActions)
                .FirstOrDefaultAsync(t => t.TransitionID == transitionId) ?? throw new Exception("Transition not found");
            var action = await _context.WorkflowActions.FindAsync(actionId) ?? throw new Exception("Action not found");
            var transitionAction = new TransitionAction
            {
                TransitionID = transitionId,
                ActionID = actionId,
                Transition = transition,
                Action = action
            };
            _context.TransitionActions.Add(transitionAction);
            await _context.SaveChangesAsync();
            transition.TransitionActions.Add(transitionAction);
        }

        public async Task<Activity> AddActivity(int processId, string name, string description, int activityTypeId)
        {
            var process = await _context.Processes
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.ProcessID == processId);
            if (process == null) throw new Exception("Process not found");

            var activityType = await _context.ActivityTypes.FindAsync(activityTypeId);
            if (activityType == null) throw new Exception("ActivityType not found");

            var activity = new Activity
            {
                ProcessID = processId,
                Name = name,
                Description = description,
                ActivityTypeID = activityTypeId,
                ActivityType = activityType,
                TransitionActivities = [],
                ActivityTargets = [],
                Process = process
            };
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            process.Activities.Add(activity);
            activityType.Activities.Add(activity);
            return activity;
        }

        public async Task AddTransitionActivity(int transitionId, int activityId)
        {
            var transition = await _context.Transitions
                .Include(t => t.TransitionActivities)
                .FirstOrDefaultAsync(t => t.TransitionID == transitionId);
            if (transition == null) throw new Exception("Transition not found");

            var activity = await _context.Activities.FindAsync(activityId);
            if (activity == null) throw new Exception("Activity not found");

            var transitionActivity = new TransitionActivity
            {
                TransitionID = transitionId,
                ActivityID = activityId,
                Transition = transition,
                Activity = activity
            };
            _context.TransitionActivities.Add(transitionActivity);
            await _context.SaveChangesAsync();
            transition.TransitionActivities.Add(transitionActivity);
        }

        public async Task<Group> CreateGroup(int processId, string name)
        {
            var process = await _context.Processes.FindAsync(processId);
            if (process == null) throw new Exception("Process not found");

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

        public async Task AddGroupMember(int groupId, int userId)
        {
            var group = await _context.Groups
                .Include(g => g.GroupMembers)
                .FirstOrDefaultAsync(g => g.GroupID == groupId);
            if (group == null) throw new Exception("Group not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var groupMember = new GroupMember
            {
                GroupID = groupId,
                UserID = userId,
                Group = group,
                User = user
            };
            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();
            group.GroupMembers.Add(groupMember);
            user.GroupMembers.Add(groupMember);
        }

        public async Task AddActionTarget(int actionId, int targetId, int groupId)
        {
            var action = await _context.WorkflowActions
                .Include(a => a.ActionTargets)
                .FirstOrDefaultAsync(a => a.WorkflowActionID == actionId);
            if (action == null) throw new Exception("Action not found");

            var target = await _context.Targets.FindAsync(targetId);
            if (target == null) throw new Exception("Target not found");

            var group = await _context.Groups.FindAsync(groupId);
            if (group == null) throw new Exception("Group not found");

            var actionTarget = new ActionTarget
            {
                ActionID = actionId,
                TargetID = targetId,
                GroupID = groupId,
                Action = action,
                Target = target,
                Group = group
            };
            _context.ActionTargets.Add(actionTarget);
            await _context.SaveChangesAsync();
            action.ActionTargets.Add(actionTarget);
            target.ActionTargets.Add(actionTarget);
            group.ActionTargets.Add(actionTarget);
        }

        public async Task AddActivityTarget(int activityId, int targetId, int groupId)
        {
            var activity = await _context.Activities
                .Include(a => a.ActivityTargets)
                .FirstOrDefaultAsync(a => a.ActivityID == activityId);
            if (activity == null) throw new Exception("Activity not found");

            var target = await _context.Targets.FindAsync(targetId);
            if (target == null) throw new Exception("Target not found");

            var group = await _context.Groups.FindAsync(groupId);
            if (group == null) throw new Exception("Group not found");

            var activityTarget = new ActivityTarget
            {
                ActivityID = activityId,
                TargetID = targetId,
                GroupID = groupId,
                Activity = activity,
                Target = target,
                Group = group
            };
            _context.ActivityTargets.Add(activityTarget);
            await _context.SaveChangesAsync();
            activity.ActivityTargets.Add(activityTarget);
            target.ActivityTargets.Add(activityTarget);
            group.ActivityTargets.Add(activityTarget);
        }

        public async Task<List<State>> GetProcessStates(int processId)
        {
            var process = await _context.Processes
                .Include(p => p.States)
                .ThenInclude(s => s.StateType)
                .FirstOrDefaultAsync(p => p.ProcessID == processId);
            if (process == null) throw new Exception("Process not found");
            return [.. process.States];
        }

        public async Task<Request> CreateRequest(int processId, string title, Dictionary<string, string> data, string? initialStateName = null, int? userId = null)
        {
            // await InitializeData();

            var process = await _context.Processes
                .Include(p => p.States)
                .ThenInclude(s => s.StateType)
                .Where(q => q.ProcessID == processId)
                .FirstOrDefaultAsync();
            if (process == null) throw new Exception("No process found");
            State? initialState;
            if(!string.IsNullOrEmpty(initialStateName))
            {
                initialState = process.States.FirstOrDefault(s => s.Name.Normalize(NormalizationForm.FormD) == initialStateName.Normalize(NormalizationForm.FormD));
                if (initialState == null) throw new Exception("Initial state not found");
            }
            else
            {
                if (process.States.Count <= 2)
                {
                    //Mặc định cho hoàn thành, nếu có rule thì tự set thất bại bỏ skip là dc
                    initialState = process.States.OrderBy(q => q.StateID).Skip(1).First();
                }
                else
                {
                    initialState = process.States.OrderBy(q => q.StateID).Skip(2).First();
                }
            }
                
            User? user;
            if(userId != null)
            {
                user = await _context.Users.FindAsync(userId);
                if (user == null) throw new Exception("User not found");
            }
            var request = new Request
            {
                Title = title,
                DateRequested = DateTime.UtcNow, 
                UserID = userId,
                CurrentStateID = initialState.StateID,
                CurrentState = initialState,
                ProcessId = processId,
                RequestNotes = [],
                RequestData = [],
                RequestFiles = [],
                RequestStakeholders = [],
                RequestActions = []
            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            foreach (var item in data)
            {
                request.RequestData.Add(new RequestData { RequestID = request.RequestID, Name = item.Key, Value = item.Value, Request = request });
            }
            await _context.SaveChangesAsync();
            //user?.Requests.Add(request);

            return request;
        }

        public async Task PerformAction(int requestId, int userId)
        {
            var request = await _context.Requests
                .Include(r => r.CurrentState)
                .Include(r => r.RequestActions)
                .FirstOrDefaultAsync(r => r.RequestID == requestId);
            if (request == null) throw new Exception("Request not found");
            var transition = await _context.Transitions.Include(t => t.NextState)
                .Include(t => t.TransitionActions)
                .ThenInclude(ta => ta.Action)
                .Include(t => t.TransitionActivities)
                .ThenInclude(ta => ta.Activity)
                .FirstOrDefaultAsync(t => t.CurrentStateID == request.CurrentStateID);
            var states = await _context.States
                .Include(s => s.StateType)
                .OrderBy(s => s.StateOrder)
                .Where(s => s.ProcessID == request.ProcessId)
                .ToListAsync();
            //var transition = await _context.Transitions
            //    .Include(t => t.NextState)
            //    .Include(t => t.TransitionActions)
            //    .ThenInclude(ta => ta.Action)
            //    .Include(t => t.TransitionActivities)
            //    .ThenInclude(ta => ta.Activity)
            //    .FirstOrDefaultAsync(t => t.TransitionID == transitionId && t.CurrentStateID == request.CurrentStateID);
            if (transition == null) throw new Exception("Invalid transition for current state");

            var nextState = await _context.States.FindAsync(transition.NextStateID);
            if (nextState != null)
            {
                if (nextState.IsFinal || transition.IsFinal)
                {
                    if (nextState.Name == "Completed" && !IsAllStepsApproved(request, states))
                        throw new Exception("Cannot complete until all steps are approved");
                }
                request.CurrentStateID = transition.NextStateID;
                request.CurrentState = nextState;
                _context.Requests.Update(request);

                // var requestAction = new RequestAction
                // {
                //     RequestID = requestId,
                //     TransitionID = transition.TransitionID,
                //     // ActionID = transition.TransitionActions.FirstOrDefault()?.ActionID ?? 0,
                //     IsActive = true,
                //     IsComplete = nextState.IsFinal,
                //     IsFinal = transition.IsFinal,
                //     Request = request,
                //     Transition = transition
                // };
                // _context.RequestActions.Add(requestAction);
                // request.RequestActions.Add(requestAction);
            }
            // Thực thi các TransitionActions và TransitionActivities
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

        private bool IsAllStepsApproved(Request request, List<State> states)
        {
            if (request.CurrentStateID == states.Last().StateID)
            {
                return true; 
            }
            return request.RequestActions.All(a => a.IsComplete);
        }

        public async Task AddRequestNote(int requestId, int userId, string note)
        {
            var request = await _context.Requests
                .Include(r => r.RequestNotes)
                .FirstOrDefaultAsync(r => r.RequestID == requestId);
            if (request == null) throw new Exception("Request not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

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

        public async Task AddRequestFile(int requestId, int userId, string fileName, byte[] fileContent, string mimeType)
        {
            var request = await _context.Requests
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(r => r.RequestID == requestId);
            if (request == null) throw new Exception("Request not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

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

        public async Task AddRequestStateHolder(int requestId, int userId)
        {
            var request = await _context.Requests
                .Include(r => r.RequestStakeholders)
                .FirstOrDefaultAsync(r => r.RequestID == requestId);
            if (request == null) throw new Exception("Request not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var requestStateHolder = new RequestStakeholder
            {
                RequestID = requestId,
                UserID = userId,
                Request = request,
                User = user
            };
            _context.RequestStakeholder.Add(requestStateHolder);
            request.RequestStakeholders.Add(requestStateHolder);
            user.RequestStakeholders.Add(requestStateHolder);
            await _context.SaveChangesAsync();
        }

        public async Task AddCustomEntity(string entityType, Dictionary<string, object> data)
        {
            var customEntity = new CustomEntity
            {
                EntityType = entityType,
                Data = System.Text.Json.JsonSerializer.Serialize(data),
                ProcessID = data.ContainsKey("ProcessID") ? (int?)data["ProcessID"] : null
            };
            _context.CustomEntities.Add(customEntity);
            await _context.SaveChangesAsync();
        }
    }
}