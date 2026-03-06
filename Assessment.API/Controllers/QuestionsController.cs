using Assessment.API.Common;
using Assessment.Application.DTOs.Questions;
using Assessment.Application.Interfaces;
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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid? testId = null)
        {
            try
            {
                List<QuestionResponseDto> list = testId.HasValue
                    ? await _questions.GetQuestionsForTestAsync(testId.Value)
                    : await _questions.GetQuestionsAsync();

                return Ok(ApiResponse<List<QuestionResponseDto>>.Success(list));
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