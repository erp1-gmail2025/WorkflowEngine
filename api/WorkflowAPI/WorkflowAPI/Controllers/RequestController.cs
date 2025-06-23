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
    public class RequestController(WorkflowEngine engine) : ControllerBase
    {
        private readonly WorkflowEngine _engine = engine;


        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestDto dto)
        {
            var request = await _engine.CreateRequest(dto.ProcessId, dto.UserId, dto.Title, dto.Data, dto.InitialStateName);
            return Ok(new
            {
                request.RequestID,
                request.Title,
                request.CurrentStateID,
                CurrentStateName = request.CurrentState.Name
            });
        }

        [HttpGet("{processId}")]
        public async Task<IActionResult> GetRequestByProcess(int processId)
        {
            var requests = await _engine.GetRequestsByProcessId(processId);
            return Ok(requests);
        }

        [HttpPost("{requestId}/action")]
        public async Task<IActionResult> PerformAction(int requestId, [FromBody] PerformActionDto dto)
        {
            await _engine.PerformAction(requestId, dto.TransitionId, dto.UserId);
            return Ok();
        }

        [HttpPost("{requestId}/notes")]
        public async Task<IActionResult> AddRequestNote(int requestId, [FromBody] AddRequestNoteDto dto)
        {
            await _engine.AddRequestNote(requestId, dto.UserId, dto.Note);
            return Ok();
        }

        [HttpPost("{requestId}/files")]
        public async Task<IActionResult> AddRequestFile(int requestId, [FromBody] AddRequestFileDto dto)
        {
            await _engine.AddRequestFile(requestId, dto.UserId, dto.FileName, dto.FileContent, dto.MimeType);
            return Ok();
        }

        [HttpPost("{requestId}/stakeholders")]
        public async Task<IActionResult> AddRequestStakeholder(int requestId, [FromBody] AddRequestStakeholderDto dto)
        {
            await _engine.AddRequestStakeholder(requestId, dto.UserId);
            return Ok();
        }
    }
}