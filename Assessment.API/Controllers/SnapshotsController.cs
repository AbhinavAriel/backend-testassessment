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

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SnapshotResponseDto>>> Upload(UploadSnapshotRequestDto dto)
        {
            var result = await _service.UploadAsync(dto);

            return Ok(ApiResponse<SnapshotResponseDto>.Success(result, "Snapshot uploaded"));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{testId:guid}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SnapshotResponseDto>>>> GetByTestId(Guid testId)
        {
            var result = await _service.GetByTestIdAsync(testId);

            return Ok(ApiResponse<IEnumerable<SnapshotResponseDto>>.Success(result));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("image/{snapshotId:guid}")]
        public async Task<IActionResult> GetImage(Guid snapshotId)
        {
            var (bytes, contentType) = await _service.GetImageAsync(snapshotId);

            return File(bytes, contentType);
        }
    }
}