using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assessment.Application.DTOs.Snapshots;

namespace Assessment.Application.Interfaces
{
    public interface ISnapshotService
    {
       
        Task<SnapshotResponseDto> UploadAsync(UploadSnapshotRequestDto dto);

        Task<List<SnapshotResponseDto>> GetByTestIdAsync(Guid testId);
    }
}