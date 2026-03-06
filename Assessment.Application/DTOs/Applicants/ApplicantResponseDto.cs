namespace Assessment.Application.DTOs.Applicants
{
    public class ApplicantResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string MobileNumber { get; set; } = "";
    }
}