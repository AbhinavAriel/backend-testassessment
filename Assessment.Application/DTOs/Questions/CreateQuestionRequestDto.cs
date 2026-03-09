using System.ComponentModel.DataAnnotations;

namespace Assessment.Application.DTOs.Questions
{
    public class CreateQuestionRequestDto
    {
        [Required]
        public string Text { get; set; } = "";

        [Required]
        public Guid TechStackId { get; set; }

        [Required]
        public string Level { get; set; } = "";

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        public List<CreateQuestionOptionDto> Options { get; set; } = new();
    }

    public class CreateQuestionOptionDto
    {
        [Required]
        public string Text { get; set; } = "";

        public bool IsCorrect { get; set; }
    }
}