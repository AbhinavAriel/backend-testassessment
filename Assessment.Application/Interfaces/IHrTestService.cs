using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assessment.Application.DTOs.Hr;
using Assessment.Application.DTOs.Questions;

namespace Assessment.Application.Interfaces
{
    public interface IHrTestService
    {
        Task<HrMetaDto> GetMetaAsync();

        Task<PagedResultDto<HrTestRowDto>> GetListPagedAsync(int page, int pageSize);

        Task<HrTestRowDto> CreateAsync(CreateHrTestRequestDto dto);

        Task<HrTestDetailDto> GetByIdAsync(Guid testId);

        Task<object> GetByTokenAsync(string token);

        Task<HrApplicantDto> GetApplicantByIdAsync(Guid applicantId);

        Task<List<QuestionResponseDto>> GetQuestionsForTestAsync(Guid testId);

        Task SubmitTestAsync(Guid testId);

        Task<HrTestReportDto> GetReportAsync(Guid testId);

        Task DeleteTestAsync(Guid testId);

        // keep if you want it (you can still throw NotImplemented)
        Task<HrTestRowDto> UpdateAsync(Guid testId, UpdateHrTestRequestDto dto);

        Task RejectTestAsync(Guid testId);
    }
}