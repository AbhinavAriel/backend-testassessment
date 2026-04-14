namespace Assessment.Application.DTOs.Hr
{
    public class BeginTestResultDto
    {
        public Guid TestId { get; init; }
        public Guid ApplicantId { get; init; }
        public int DurationMinutes { get; init; }
    }
}