using HrSystem.Application;
using HrSystem.Application.Models.Loans;

namespace HrSystem.Application.Features.Loans;

public sealed class LoanHandler(ILoanService service)
{
    public Task<IReadOnlyCollection<LoanDto>> GetPendingAsync(CancellationToken ct) => service.GetPendingAsync(ct);
    public Task<int> CreateAsync(CreateLoanRequest request, CancellationToken ct) => service.CreateAsync(request, ct);
    public Task DecideAsync(int id, bool approve, CancellationToken ct) => service.DecideAsync(id, approve, ct);
}
