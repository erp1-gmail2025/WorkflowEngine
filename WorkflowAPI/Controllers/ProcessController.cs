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
        public async Task<IActionResult> InitializeData([FromBody] Dictionary<string, object> customData)
        {
            await _engine.InitializeData(customData);
            return Ok("Data initialized");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcess([FromBody] CreateProcessDto dto)
        {
            var process = await _engine.CreateProcess(dto.AdminId, dto.UserId, dto.Name);
            return Ok(process);
        }

        [HttpPost("{processId}/states")]
        public async Task<IActionResult> AddState(int processId, [FromBody] AddStateDto dto)
        {
            var state = await _engine.AddState(processId, dto.Name, dto.Description, dto.StateTypeId);
            return Ok(state);
        }

        [HttpPost("{processId}/transitions")]
        public async Task<IActionResult> AddTransition(int processId, [FromBody] AddTransitionDto dto)
        {
            var transition = await _engine.AddTransition(processId, dto.CurrentStateName, dto.NextStateName, dto.IsFinal);
            return Ok(transition);
        }

        [HttpPost("actions")]
        public async Task<IActionResult> CreateAction([FromBody] CreateActionDto dto)
        {
            var action = await _engine.CreateAction(dto.ActionTypeId, dto.ProcessId, dto.Name, dto.Description);
            return Ok(action);
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
            var activity = await _engine.AddActivity(processId, dto.Name, dto.Description, dto.ActivityTypeId);
            return Ok(activity);
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
            return Ok(group);
        }

        [HttpPost("groups/{groupId}/members")]
        public async Task<IActionResult> AddGroupMember(int groupId, [FromBody] AddGroupMemberDto dto)
        {
            await _engine.AddGroupMember(groupId, dto.UserId);
            return Ok();
        }

        [HttpPost("actions/{actionId}/targets")]
        public async Task<IActionResult> AddActionTarget(int actionId, [FromBody] AddActionTargetDto dto)
        {
            await _engine.AddActionTarget(actionId, dto.TargetId, dto.GroupId);
            return Ok();
        }

        [HttpPost("activities/{activityId}/targets")]
        public async Task<IActionResult> AddActivityTarget(int activityId, [FromBody] AddActivityTargetDto dto)
        {
            await _engine.AddActivityTarget(activityId, dto.TargetId, dto.GroupId);
            return Ok();
        }

        [HttpPost("custom")]
        public async Task<IActionResult> AddCustomEntity([FromBody] AddCustomEntityDto dto)
        {
            await _engine.AddCustomEntity(dto.EntityType, dto.Data);
            return Ok();
        }

        [HttpGet("{processId}/states")]
        public async Task<IActionResult> GetProcessStates(int processId)
        {
            var states = await _engine.GetProcessStates(processId);
            return Ok(states);
        }
    }
}