using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorkflowAPI.Service;
using WorkflowAPI.ViewModel;

namespace WorkflowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly WorkflowEngine _engine;
        public ProcessController(WorkflowEngine engine)
        {
            _engine = engine;
        }

        [HttpPost("init")]
        public async Task<IActionResult> InitializeData()
        {
            await _engine.InitializeData();
            return Ok("Data initialized");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcess([FromBody] CreateProcessDto dto)
        {
            var process = await _engine.CreateProcess(dto.AdminId, dto.Name);
            return Ok(new { process.ProcessID, process.Name });
        }

        [HttpPost("{processId}/states")]
        public async Task<IActionResult> AddState(int processId, [FromBody] AddStateDto dto)
        {
            var state = await _engine.AddState(processId, dto.Name, dto.Description, dto.StateTypeId);
            return Ok(new { state.StateID, state.Name, state.Description, state.StateOrder });
        }

        [HttpPost("{processId}/transitions")]
        public async Task<IActionResult> AddTransition(int processId, [FromBody] AddTransitionDto dto)
        {
            var transition = await _engine.AddTransition(processId, dto.CurrentStateName, dto.NextStateName, dto.IsFinal);
            return Ok(new { transition.TransitionID, transition.CurrentStateID, transition.NextStateID });
        }

        [HttpPost("actions")]
        public async Task<IActionResult> CreateAction([FromBody] CreateActionDto dto)
        {
            var action = await _engine.CreateAction(dto.ActionTypeId, dto.ProcessId, dto.Name, dto.Description, dto.TargetId, dto.GroupId);
            return Ok(new { action.WorkflowActionID, action.Name, action.Description });
        }

        [HttpPost("transitions/{transitionId}/actions")]
        public async Task<IActionResult> AddTransitionAction(int transitionId, [FromBody] AddTransitionActionDto dto)
        {
            await _engine.AddTransitionAction(transitionId, dto.ActionId);
            return Ok();
        }

        [HttpPost("{processId}/activities")]
        public async Task<IActionResult> AddActivity(int processId, [FromBody] AddActivityDto dto)
        {
            var activity = await _engine.AddActivity(processId, dto.Name, dto.Description, dto.ActivityTypeId, dto.TargetId, dto.GroupId);
            return Ok(new { activity.ActivityID, activity.Name, activity.Description });
        }

        [HttpPost("transitions/{transitionId}/activities")]
        public async Task<IActionResult> AddTransitionActivity(int transitionId, [FromBody] AddTransitionActivityDto dto)
        {
            await _engine.AddTransitionActivity(transitionId, dto.ActivityId);
            return Ok();
        }

        [HttpPost("{processId}/groups")]
        public async Task<IActionResult> CreateGroup(int processId, [FromBody] CreateGroupDto dto)
        {
            var group = await _engine.CreateGroup(processId, dto.Name);
            return Ok(new { group.GroupID, group.Name });
        }

        [HttpPost("groups/{groupId}/members")]
        public async Task<IActionResult> AddGroupMember(int groupId, [FromBody] AddGroupMemberDto dto)
        {
            await _engine.AddGroupMember(groupId, dto.UserId);
            return Ok();
        }

        [HttpGet("{processId}/states")]
        public async Task<IActionResult> GetProcessStates(int processId)
        {
            var states = await _engine.GetProcessStates(processId);
            return Ok(states.Select(s => new
            {
                s.StateID,
                s.Name,
                s.Description,
                s.IsFinal,
                s.StateOrder
            }));
        }

        [HttpGet("{processId}/transitions")]
        public async Task<IActionResult> GetTransitions(int processId)
        {
            var process = await _engine.GetTransitions(processId);
            if (process == null) return NotFound();
            return Ok(process.Transitions.Select(t => new
            {
                t.TransitionID,
                t.CurrentStateID,
                t.NextStateID,
                CurrentStateName = t.CurrentState?.Name,
                NextStateName = t.NextState?.Name,
                t.IsFinal
            }));
        }

        [HttpGet("requests/{requestId}/next-states")]
        public async Task<IActionResult> GetAvailableNextStates(int requestId)
        {
            var nextStates = await _engine.GetAvailableNextStates(requestId);
            return Ok(nextStates.Select(s => new
            {
                s.StateID,
                s.Name,
                s.Description,
                s.StateOrder,
                s.IsFinal
            }));
        }

        [HttpPost("requests/{requestId}/action/by-state-id")]
        public async Task<IActionResult> PerformActionByNextStateId(int requestId, [FromBody] PerformActionByStateIdDto dto)
        {
            await _engine.PerformActionByNextStateId(requestId, dto.NextStateId, dto.UserId, dto.FailureReason);
            return Ok();
        }

        [HttpGet("processes")]
        public async Task<IActionResult> GetProcesses()
        {
            var processes = await _engine.GetProcesses();
            return Ok(processes);
        }
    }
}