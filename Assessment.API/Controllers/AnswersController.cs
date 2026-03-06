using Assessment.API.Common;
using Assessment.Application.DTOs.Answers;
using Assessment.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _answers;

        public AnswersController(IAnswerService answers)
        {
            _answers = answers;
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] SubmitAnswerDto dto)
        {
            try
            {
                var result = await _answers.SubmitAnswerAsync(dto);
                return Ok(ApiResponse<object>.Success(result, "Saved"));
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
    }
}