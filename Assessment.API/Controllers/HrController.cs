using Assessment.API.Common;
using Assessment.Application.DTOs.Hr;
using Assessment.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HrController : ControllerBase
    {
        private readonly IHrTestService _service;

        public HrController(IHrTestService service)
        {
            _service = service;
        }

        [HttpGet("meta")]
        public async Task<IActionResult> Meta()
        {
            var data = await _service.GetMetaAsync();
            return Ok(ApiResponse<object>.Success(data));
        }

        [HttpGet("tests")]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetListPagedAsync(page, pageSize);
            return Ok(ApiResponse<object>.Success(data));
        }

        [HttpGet("tests/{testId:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid testId)
        {
            try
            {
                var data = await _service.GetByIdAsync(testId);
                return Ok(ApiResponse<object>.Success(data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // ✅ Token route for public link (enforces expiry)
        [HttpGet("tests/by-token/{token}")]
        public async Task<IActionResult> GetByToken([FromRoute] string token)
        {
            try
            {
                var data = await _service.GetByTokenAsync(token);
                return Ok(ApiResponse<object>.Success(data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpPost("tests")]
        public async Task<IActionResult> Create([FromBody] CreateHrTestRequestDto dto)
        {
            var data = await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.Success(data, "Test created"));
        }

        [HttpPost("tests/{testId:guid}/submit")]
        public async Task<IActionResult> Submit([FromRoute] Guid testId)
        {
            try
            {
                await _service.SubmitTestAsync(testId);
                return Ok(ApiResponse<object>.Success(null, "Submitted"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpGet("tests/{testId:guid}/report")]
        public async Task<IActionResult> Report([FromRoute] Guid testId)
        {
            var data = await _service.GetReportAsync(testId);
            return Ok(ApiResponse<object>.Success(data));
        }

        [HttpDelete("tests/{testId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid testId)
        {
            await _service.DeleteTestAsync(testId);
            return Ok(ApiResponse<object>.Success(null, "Deleted"));
        }
    }
}