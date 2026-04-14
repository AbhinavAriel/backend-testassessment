using Assessment.API.Common;
using Assessment.Application.DTOs.Questions;
using Assessment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questions;

        public QuestionsController(IQuestionService questions)
        {
            _questions = questions;
        }

        [Authorize(Roles = "Candidate")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid? testId = null)
        {
            try
            {
                if (testId.HasValue)
                {
                    // Candidate can only fetch questions for their own test
                    var claimedTestId = User.FindFirst("testId")?.Value;
                    if (claimedTestId == null || !Guid.TryParse(claimedTestId, out var claimedGuid) || claimedGuid != testId.Value)
                        return Forbid();

                    var list = await _questions.GetQuestionsForTestAsync(testId.Value);
                    return Ok(ApiResponse<List<QuestionResponseDto>>.Success(list));
                }

                // Listing all questions requires Admin
                if (!User.IsInRole("Admin"))
                    return Forbid();

                var all = await _questions.GetQuestionsAsync();
                return Ok(ApiResponse<List<QuestionResponseDto>>.Success(all));
            }
            catch (ArgumentException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
            catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
            catch (InvalidOperationException ex) { return Conflict(ApiResponse<object>.Fail(ex.Message)); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionRequestDto dto)
        {
            var data = await _questions.CreateAsync(dto);
            return Ok(ApiResponse<QuestionResponseDto>.Success(data, "Question created successfully."));
        }
    }
}