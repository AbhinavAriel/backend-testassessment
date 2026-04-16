using Assessment.API.Common;
using Assessment.Application.DTOs.Hr;
using Assessment.Application.DTOs.Hr.Meta;
using Assessment.Application.DTOs.Hr.Requests;
using Assessment.Application.DTOs.Hr.Test;
using Assessment.Application.Interfaces;
using Assessment.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    /// <summary>
    /// Admin-only endpoints for managing HR tests.
    /// Every action in this controller requires a valid Admin JWT.
    /// </summary>
    [ApiController]
    [Route("api/Hr")]
    [Authorize(Roles = "Admin")]
    public class HrController : ControllerBase
    {
        private readonly IHrTestService _service;

        public HrController(IHrTestService service)
        {
            _service = service;
        }

        // GET api/Hr/meta
        [HttpGet("meta")]
        public async Task<IActionResult> Meta()
        {
            var data = await _service.GetMetaAsync();
            return Ok(ApiResponse<object>.Success(data));
        }

        // GET api/Hr/tests
        [HttpGet("tests")]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetListPagedAsync(page, pageSize);
            return Ok(ApiResponse<object>.Success(data));
        }

        // POST api/Hr/tests
        [HttpPost("tests")]
        public async Task<IActionResult> Create([FromBody] CreateHrTestRequestDto dto)
        {
            var data = await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.Success(data, "Test created"));
        }

        // PUT api/Hr/tests/{testId}
        [HttpPut("tests/{testId:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid testId, [FromBody] UpdateHrTestRequestDto dto)
        {
            var data = await _service.UpdateAsync(testId, dto);
            return Ok(ApiResponse<object>.Success(data, "Test updated"));
        }

        // GET api/Hr/tests/{testId}/report
        [HttpGet("tests/{testId:guid}/report")]
        public async Task<IActionResult> Report([FromRoute] Guid testId)
        {
            var data = await _service.GetReportAsync(testId);
            return Ok(ApiResponse<object>.Success(data));
        }

        // DELETE api/Hr/tests/{testId}
        [HttpDelete("tests/{testId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid testId)
        {
            await _service.DeleteTestAsync(testId);
            return Ok(ApiResponse<object>.Success(null, "Deleted"));
        }

        // PATCH api/Hr/tests/{testId}/reject
        [HttpPatch("tests/{testId:guid}/reject")]
        public async Task<IActionResult> Reject([FromRoute] Guid testId, [FromBody] RejectHrTestRequestDto dto)
        {
            await _service.RejectTestAsync(testId, dto?.CancellationReason ?? "");
            return Ok(ApiResponse<object>.Success(null, "Candidate result cancelled."));
        }
    }
}