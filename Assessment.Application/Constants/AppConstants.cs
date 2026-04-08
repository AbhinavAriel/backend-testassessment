namespace Assessment.Application.Constants
{
    public static class TestStatus
    {
        public const string Created = "Created";
        public const string Submitted = "Submitted";
    }

    public static class QuestionLevelLabels
    {
        public const string Beginner = "Beginner";
        public const string Intermediate = "Intermediate";
        public const string Professional = "Professional";

        public static readonly IReadOnlyList<string> All =
            new[] { Beginner, Intermediate, Professional };
    }

    public static class ScoringConstants
    {
        public const decimal PassThresholdPercent = 75m;
        public const int TokenExpiryDays = 1;
    }

    public static class PaginationDefaults
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
    }
}
