using HrSystem.Application.Models.Loans;

namespace HrSystem.Application.Features.Loans.Contracts;

public interface ILoanService
{
    Task<IReadOnlyCollection<LoanDto>> GetPendingAsync(CancellationToken ct);
    Task<int> CreateAsync(CreateLoanRequest request, CancellationToken ct);
    Task DecideAsync(int id, bool approve, CancellationToken ct);
}
