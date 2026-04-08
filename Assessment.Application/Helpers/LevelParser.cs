using Assessment.Application.Constants;
using Assessment.Domain.Entities;

namespace Assessment.Application.Helpers
{
    public static class LevelParser
    {
        public static QuestionLevel Parse(string? level)
        {
            return (level ?? "").Trim().ToLowerInvariant() switch
            {
                "beginner" => QuestionLevel.Beginner,
                "intermediate" => QuestionLevel.Intermediate,
                "professional" => QuestionLevel.Professional,
                _ => throw new ArgumentException(
                    $"Invalid level '{level}'. " +
                    $"Use {QuestionLevelLabels.Beginner} / " +
                    $"{QuestionLevelLabels.Intermediate} / " +
                    $"{QuestionLevelLabels.Professional}.")
            };
        }
    }
}
