using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Application.DTOs.Hr.Requests
{
    public class UpdateHrTestRequestDto
    {
        public string FullName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        /// <summary>Each selected tech stack with its own difficulty level.</summary>
        public List<TechStackLevelDto> TechStacks { get; set; } = new();

        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }
    }
}
