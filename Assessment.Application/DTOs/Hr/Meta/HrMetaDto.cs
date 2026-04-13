using Assessment.Application.Constants;

namespace Assessment.Application.DTOs.Hr.Meta
{
    public class HrMetaDto
    {
        public List<HrTechStackDto> TechStacks { get; set; } = new();
        public List<string> Levels { get; set; } = QuestionLevelLabels.All.ToList();
    }
}
