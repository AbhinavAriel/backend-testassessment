namespace Assessment.Application.DTOs.Hr
{
    public class CandidateTokenResponseDto
    {
        public string Token { get; init; } = "";
        public DateTime ExpiresAtUtc { get; init; }
        public Guid TestId { get; init; }
    }
}