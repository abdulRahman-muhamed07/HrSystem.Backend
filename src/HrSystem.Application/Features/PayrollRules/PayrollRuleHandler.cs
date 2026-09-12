using HrSystem.Application;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Features.PayrollRules;

public sealed record PayrollRuleDto(int Id, string Name, decimal Value, bool IsPercentage, bool IsActive);
public sealed record CreatePayrollRuleRequest(string Name, decimal Value, bool IsPercentage);

public sealed class PayrollRuleHandler(IRepository<PayrollRule> repository, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyCollection<PayrollRuleDto>> GetAsync(CancellationToken ct) =>
        await repository.QueryAsync(x => new PayrollRuleDto(x.Id, x.Name, x.Value, x.IsPercentage, x.IsActive), take: 500, cancellationToken: ct);

    public async Task<int> CreateAsync(CreatePayrollRuleRequest request, CancellationToken ct)
    {
        var rule = new PayrollRule(request.Name, request.Value, request.IsPercentage);
        await repository.AddAsync(rule, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return rule.Id;
    }
}
