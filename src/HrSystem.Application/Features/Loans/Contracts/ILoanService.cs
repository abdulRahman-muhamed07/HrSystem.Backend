namespace HrSystem.Application;

public interface ILoanService
{
    Task<int> CreateAsync(CreateLoanRequest request, CancellationToken ct);
    Task<IReadOnlyCollection<LoanDto>> GetPendingAsync(CancellationToken ct);
    Task DecideAsync(int id, bool approve, CancellationToken ct);
}
