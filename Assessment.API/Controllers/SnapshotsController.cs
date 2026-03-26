using Assessment.API.Common;
using Assessment.Application.DTOs.Snapshots;
using Assessment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    /// <summary>
    /// Handles proctoring snapshot upload (candidate, anonymous) and
    /// retrieval (HR admin, authorized).
    ///
    /// Routes match the frontend constants exactly:
    ///   POST  api/Snapshots          ← uploadSnapshot()
    ///   GET   api/Snapshots/{testId} ← getSnapshots(testId)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SnapshotsController : ControllerBase
    {
        private readonly ISnapshotService _snapshots;

        public SnapshotsController(ISnapshotService snapshots)
        {
            _snapshots = snapshots;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Upload([FromBody] UploadSnapshotRequestDto dto)
        {
            try
            {
                var result = await _snapshots.UploadAsync(dto);
                return Ok(ApiResponse<object>.Success(result, "Snapshot saved"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{testId:guid}")]
        public async Task<IActionResult> GetByTestId([FromRoute] Guid testId)
        {
            try
            {
                var result = await _snapshots.GetByTestIdAsync(testId);
                return Ok(ApiResponse<object>.Success(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}