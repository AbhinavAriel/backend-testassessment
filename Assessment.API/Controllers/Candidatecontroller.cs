using Assessment.API.Common;
using Assessment.Application.DTOs.Hr;
using Assessment.Application.Interfaces;
using Assessment.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    
    [ApiController]
    [Route("api/Hr")]
    [Authorize(Roles = "Candidate")]
    public class CandidateController : ControllerBase
    {
        private readonly IHrTestService _service;
        private readonly CandidateTokenGenerator _candidateTokenGenerator;

        public CandidateController(IHrTestService service, CandidateTokenGenerator candidateTokenGenerator)
        {
            _service = service;
            _candidateTokenGenerator = candidateTokenGenerator;
        }

        [AllowAnonymous]
        [HttpGet("tests/by-token/{token}")]
        public async Task<IActionResult> GetByToken([FromRoute] string token)
        {
            try
            {
                var data = await _service.GetByTokenAsync(token);
                return Ok(ApiResponse<object>.Success(data));
            }
            catch (ArgumentException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
            catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
            catch (InvalidOperationException ex) { return Conflict(ApiResponse<object>.Fail(ex.Message)); }
        }

        [AllowAnonymous]
        [HttpPost("tests/{testId:guid}/begin")]
        public async Task<IActionResult> Begin([FromRoute] Guid testId)
        {
            try
            {
                var data = await _service.BeginTestAsync(testId);

                var (token, expiresAtUtc) = _candidateTokenGenerator.Generate(
                    data.TestId,
                    data.ApplicantId,
                    data.DurationMinutes
                );

                return Ok(ApiResponse<object>.Success(new CandidateTokenResponseDto
                {
                    Token = token,
                    ExpiresAtUtc = expiresAtUtc,
                    TestId = data.TestId,
                }));
            }
            catch (ArgumentException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
            catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
            catch (InvalidOperationException ex) { return Conflict(ApiResponse<object>.Fail(ex.Message)); }
        }

        [AllowAnonymous]
        [HttpGet("tests/{testId:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid testId)
        {
            try
            {
                var data = await _service.GetByIdAsync(testId);
                return Ok(ApiResponse<object>.Success(data));
            }
            catch (ArgumentException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
            catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
            catch (InvalidOperationException ex) { return Conflict(ApiResponse<object>.Fail(ex.Message)); }
        }

        [HttpPost("tests/{testId:guid}/submit")]
        public async Task<IActionResult> Submit([FromRoute] Guid testId)
        {
            try
            {
                // Extra safety: verify the token is scoped to exactly this test.
                var claimedTestId = User.FindFirst("testId")?.Value;
                if (claimedTestId == null ||
                    !Guid.TryParse(claimedTestId, out var claimedGuid) ||
                    claimedGuid != testId)
                {
                    return Forbid();
                }

                await _service.SubmitTestAsync(testId);
                return Ok(ApiResponse<object>.Success(null, "Submitted"));
            }
            catch (ArgumentException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
            catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
            catch (InvalidOperationException ex) { return Conflict(ApiResponse<object>.Fail(ex.Message)); }
        }
    }
}