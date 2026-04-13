using Assessment.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Application.DTOs.Hr.Shared
{

    public class TechStackLevelDto
    {
        public Guid TechStackId { get; set; }
        public string Level { get; set; } = QuestionLevelLabels.Beginner;
    }
}
