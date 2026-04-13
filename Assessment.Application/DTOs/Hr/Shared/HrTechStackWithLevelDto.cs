using Assessment.Application.Constants;


namespace Assessment.Application.DTOs.Hr.Shared
{
    public class HrTechStackWithLevelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Level { get; set; } = QuestionLevelLabels.Beginner;
    }
}
