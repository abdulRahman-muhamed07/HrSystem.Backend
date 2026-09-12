using HrSystem.Application;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Features.EmployeeDocuments;

public sealed record EmployeeDocumentDto(int Id, int EmployeeId, string DocumentType, string FilePath, DateTime? ExpiryDate, DateTime UploadedAt);
public sealed record CreateEmployeeDocumentRequest(int EmployeeId, string DocumentType, string FilePath, DateTime? ExpiryDate);

public sealed class EmployeeDocumentHandler(IRepository<EmployeeDocument> repository, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyCollection<EmployeeDocumentDto>> GetAsync(int employeeId, CancellationToken ct) =>
        await repository.QueryAsync(x => new EmployeeDocumentDto(x.Id, x.EmployeeId, x.DocumentType, x.FilePath, x.ExpiryDate, x.UploadedAt), x => x.EmployeeId == employeeId, take: 500, cancellationToken: ct);

    public async Task<int> CreateAsync(CreateEmployeeDocumentRequest request, CancellationToken ct)
    {
        var document = new EmployeeDocument(request.EmployeeId, request.DocumentType, request.FilePath, request.ExpiryDate);
        await repository.AddAsync(document, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return document.Id;
    }
}
