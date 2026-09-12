namespace HrSystem.Domain.Entities;

public sealed class EmployeeDocument
{
    public int Id { get; private set; }
    public int EmployeeId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public DateTime? ExpiryDate { get; private set; }
    public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;
    public Employee? Employee { get; private set; }
    private EmployeeDocument() { }
    public EmployeeDocument(int employeeId, string documentType, string filePath, DateTime? expiryDate)
    { EmployeeId = employeeId; DocumentType = documentType.Trim(); FilePath = filePath.Trim(); ExpiryDate = expiryDate?.Date; }
}
