using Assessment.Application.DTOs.Auth;
using Assessment.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantsController : ControllerBase
    {
        private readonly IAuthService _authService;

        public ApplicantsController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApplicantDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Invalid request." });

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { error = "Full name is required." });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { error = "Email is required." });

            if (string.IsNullOrWhiteSpace(dto.MobileNumber))
                return BadRequest(new { error = "Phone number is required." });

            try
            {
                var result = await _authService.RegisterAsync(new RegisterRequestDto
                {
                    FullName = dto.Name,
                    Email = dto.Email,
                    PhoneNumber = dto.MobileNumber
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { error = msg });
            }
        }
    }

    public class CreateApplicantDto
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string MobileNumber { get; set; } = "";
    }
}