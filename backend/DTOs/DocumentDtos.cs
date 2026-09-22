using JobTracker.Api.Models;

namespace JobTracker.Api.DTOs;

public record DocumentDto(int Id, int ApplicationId, string FileName, string FilePath, DocumentType DocumentType, DateTime UploadedAt);
