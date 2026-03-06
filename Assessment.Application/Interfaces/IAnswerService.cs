using Assessment.Application.DTOs.Answers;
using System;
using System.Threading.Tasks;

namespace Assessment.Application.Interfaces
{
    public interface IAnswerService
    {
        Task<object> SubmitAnswerAsync(SubmitAnswerDto dto);
    }
}