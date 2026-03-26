using System;

namespace Assessment.Application.DTOs.Snapshots
{
    public class UploadSnapshotRequestDto
    {
        public Guid TestId { get; set; }
        public Guid ApplicantId { get; set; }

        public string ImageData { get; set; } = "";

        public DateTime CapturedAt { get; set; }
    }

    public class SnapshotResponseDto
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public Guid ApplicantId { get; set; }

        public string ImageUrl { get; set; } = "";

        public DateTime CapturedAt { get; set; }
    }
}