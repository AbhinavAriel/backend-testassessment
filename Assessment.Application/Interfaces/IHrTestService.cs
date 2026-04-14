using Assessment.Application.DTOs.Hr;
using Assessment.Application.DTOs.Hr.Meta;
using Assessment.Application.DTOs.Hr.Test;
using Assessment.Application.DTOs.Hr.Requests;
using Assessment.Application.DTOs.Hr.Applicant;
using Assessment.Application.DTOs.Questions;

namespace Assessment.Application.Interfaces
{
    public interface IHrTestService
    {
        Task<HrMetaDto> GetMetaAsync();
        Task<PagedResultDto<HrTestRowDto>> GetListPagedAsync(int page, int pageSize);
        Task<HrTestRowDto> CreateAsync(CreateHrTestRequestDto dto);
        Task<HrTestDetailDto> GetByIdAsync(Guid testId);
        Task<HrTestTokenResponseDto> GetByTokenAsync(string token);
        Task<HrApplicantDto> GetApplicantByIdAsync(Guid applicantId);
        Task<List<QuestionResponseDto>> GetQuestionsForTestAsync(Guid testId);
        Task<BeginTestResultDto> BeginTestAsync(Guid testId); 
        Task SubmitTestAsync(Guid testId);
        Task<HrTestReportDto> GetReportAsync(Guid testId);
        Task DeleteTestAsync(Guid testId);
        Task<HrTestRowDto> UpdateAsync(Guid testId, UpdateHrTestRequestDto dto);
        Task RejectTestAsync(Guid testId, string cancellationReason);
    }
}