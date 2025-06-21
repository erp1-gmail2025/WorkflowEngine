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
            var request = await _engine.CreateRequest(dto.ProcessId, dto.Title, dto.Data, dto.InitialStateName, dto.UserId);
            return Ok(request);
        }

        [HttpPost("{requestId}/action")]
        public async Task<IActionResult> PerformAction(int requestId, [FromBody] PerformActionDto dto)
        {
            await _engine.PerformAction(requestId, dto.UserId);
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
            await _engine.AddRequestFile(requestId, dto.UserId, dto.FileName, dto.FileContent, dto.MIMETYPE);
            return Ok();
        }

        [HttpPost("{requestId}/stateholders")]
        public async Task<IActionResult> AddRequestStateHolder(int requestId, [FromBody] AddRequestStateHolderDto dto)
        {
            await _engine.AddRequestStateHolder(requestId, dto.UserId);
            return Ok();
        }
    }
}