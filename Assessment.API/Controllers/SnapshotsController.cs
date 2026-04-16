using Assessment.API.Common;
using Assessment.Application.DTOs.Snapshots;
using Assessment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SnapshotsController : ControllerBase
    {
        private readonly ISnapshotService _service;

        public SnapshotsController(ISnapshotService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Candidate")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SnapshotResponseDto>>> Upload(
            [FromBody] UploadSnapshotRequestDto dto)
        {
            var claimedTestId = User.FindFirst("testId")?.Value;
            if (claimedTestId == null ||
                !Guid.TryParse(claimedTestId, out var claimedGuid) ||
                claimedGuid != dto.TestId)
            {
                return Forbid();
            }

            var result = await _service.UploadAsync(dto);
            return Ok(ApiResponse<SnapshotResponseDto>.Success(result, "Snapshot uploaded"));
        }

        // GET api/Snapshots/{testId}
        [Authorize(Roles = "Admin")]
        [HttpGet("{testId:guid}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SnapshotResponseDto>>>> GetByTestId(
            Guid testId)
        {
            var result = await _service.GetByTestIdAsync(testId);
            return Ok(ApiResponse<IEnumerable<SnapshotResponseDto>>.Success(result));
        }

        // GET api/Snapshots/image/{snapshotId}
        [Authorize(Roles = "Admin")]
        [HttpGet("image/{snapshotId:guid}")]
        public async Task<IActionResult> GetImage(Guid snapshotId)
        {
            var (bytes, contentType) = await _service.GetImageAsync(snapshotId);
            return File(bytes, contentType);
        }
    }
}