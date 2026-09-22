namespace JobTracker.Api.Models;

public class Document
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public JobApplication? Application { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
