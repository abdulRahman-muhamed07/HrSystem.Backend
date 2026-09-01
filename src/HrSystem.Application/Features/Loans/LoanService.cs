using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class LoanService(IRepository<EmployeeLoan> loans, IRepository<Employee> employees, IUnitOfWork unitOfWork, IAuditService audit, ICurrentUser currentUser) : ILoanService
{
    public async Task<int> CreateAsync(CreateLoanRequest request, CancellationToken ct)
    {
        if (request.Amount <= 0) throw new BusinessRuleException("Loan amount must be positive.");
        if (request.Installments <= 0) throw new BusinessRuleException("Installments must be positive.");
        if (await employees.GetByIdAsync(request.EmployeeId, ct) is null) throw new NotFoundException("Employee was not found.");
        var entity = new EmployeeLoan(request.EmployeeId, request.Amount, request.Installments, request.Reason);
        await loans.AddAsync(entity, ct); await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Create", nameof(EmployeeLoan), entity.Id.ToString(), null, ct); return entity.Id;
    }
    public async Task<IReadOnlyCollection<LoanDto>> GetPendingAsync(CancellationToken ct) => await loans.QueryAsync(l => new LoanDto(l.Id, l.EmployeeId, l.Employee!.FullName, l.Amount, l.Installments, l.MonthlyDeduction, l.RemainingAmount, l.Reason, l.Status), l => l.Status == LoanStatus.Pending, 0, int.MaxValue, ct);
    public async Task DecideAsync(int id, bool approve, CancellationToken ct)
    {
        var entity = await loans.GetByIdAsync(id, ct) ?? throw new NotFoundException("Loan was not found.");
        if (entity.Status != LoanStatus.Pending) throw new BusinessRuleException("Only pending loans can be decided.");
        var userId = currentUser.UserId ?? throw new BusinessRuleException("Authenticated user is required.");
        if (approve) entity.Approve(userId); else entity.Reject(userId);
        await unitOfWork.SaveChangesAsync(ct); await audit.WriteAsync(approve ? "Approve" : "Reject", nameof(EmployeeLoan), id.ToString(), null, ct);
    }
}
