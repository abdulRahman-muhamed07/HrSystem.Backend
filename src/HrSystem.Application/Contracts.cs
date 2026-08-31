using System.Linq.Expressions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application;

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(string Token, DateTime ExpiresAt, int UserId, string FullName, UserRole Role, int? EmployeeId);

public sealed record EmployeeListItem(int Id, string FullName, string Email, string JobTitle, string DepartmentName, EmploymentStatus Status, decimal Salary);
public sealed record EmployeeDetails(int Id, string FullName, string Email, string? NationalId, string? Phone, string JobTitle, int DepartmentId, string DepartmentName, EmploymentType EmploymentType, EmploymentStatus EmploymentStatus, decimal Salary, decimal HousingAllowance, decimal TransportationAllowance, decimal MealAllowance, DateTime HireDate);
public sealed record CreateEmployeeRequest(string FullName, string Email, string JobTitle, int DepartmentId, decimal Salary, DateTime HireDate, EmploymentType EmploymentType = EmploymentType.FullTime, string? Phone = null, string? Address = null);
public sealed record UpdateEmployeeRequest(string FullName, string Email, string JobTitle, int DepartmentId, decimal Salary, EmploymentType EmploymentType, EmploymentStatus EmploymentStatus, string? Phone, string? Address, decimal HousingAllowance, decimal TransportationAllowance, decimal MealAllowance);

public sealed record DepartmentDto(int Id, string Name, string? Description, int EmployeeCount);
public sealed record CreateDepartmentRequest(string Name, string? Description);

public sealed record AttendanceDto(int Id, int EmployeeId, DateTime Date, TimeOnly? CheckIn, TimeOnly? CheckOut, AttendanceStatus Status);
public sealed record CheckInRequest(int EmployeeId, TimeOnly? CheckIn = null);
public sealed record CheckOutRequest(TimeOnly? CheckOut = null);

public sealed record LeaveRequestDto(int Id, int EmployeeId, string EmployeeName, int LeaveTypeId, string LeaveTypeName, DateTime StartDate, DateTime EndDate, decimal DurationDays, string Reason, LeaveRequestStatus Status, string? RejectionReason);
public sealed record CreateLeaveRequest(int EmployeeId, int LeaveTypeId, DateTime StartDate, DateTime EndDate, string Reason);
public sealed record LeaveDecisionRequest(bool Approve, string? RejectionReason);

public sealed record OvertimeDto(int Id, int EmployeeId, string EmployeeName, DateTime Date, decimal Hours, decimal RateMultiplier, string? Reason, OvertimeStatus Status);
public sealed record CreateOvertimeRequest(int EmployeeId, DateTime Date, decimal Hours, decimal RateMultiplier = 1.25m, string? Reason = null);

public sealed record LoanDto(int Id, int EmployeeId, string EmployeeName, decimal Amount, int Installments, decimal MonthlyDeduction, decimal RemainingAmount, string? Reason, LoanStatus Status);
public sealed record CreateLoanRequest(int EmployeeId, decimal Amount, int Installments, string? Reason);

public sealed record PayrollDto(int Id, int EmployeeId, string EmployeeName, int Year, int Month, decimal GrossSalary, decimal NetSalary, decimal OvertimePay, decimal LoanDeduction, PayrollStatus Status, DateTime? PaidAt);

public sealed record DashboardDto(int EmployeeCount, int ActiveEmployeeCount, int PendingLeaves, int PendingOvertime, int PendingLoans, decimal PayrollNetThisMonth);
public sealed record UserDto(int Id, string Email, string FullName, UserRole Role, bool IsActive, int? EmployeeId);

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<TResult>> QueryAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>>? predicate = null, int skip = 0, int take = int.MaxValue, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Remove(T entity);
}

public interface IUnitOfWork { Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); }
public interface ICurrentUser { int? UserId { get; } string? UserName { get; } string? Role { get; } }
public interface ITokenService { (string Token, DateTime ExpiresAt) Create(User user); }
public interface IPasswordHasher { string Hash(string password); bool Verify(string password, string hash); }
public interface IAuditService { Task WriteAsync(string action, string entityName, string? entityId = null, string? details = null, CancellationToken cancellationToken = default); }

public interface IAuthService { Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken); }
public interface IEmployeeService { Task<PagedResult<EmployeeListItem>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct); Task<EmployeeDetails?> GetAsync(int id, CancellationToken ct); Task<int> CreateAsync(CreateEmployeeRequest request, CancellationToken ct); Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken ct); Task DeleteAsync(int id, CancellationToken ct); }
public interface IDepartmentService { Task<IReadOnlyCollection<DepartmentDto>> GetAllAsync(CancellationToken ct); Task<int> CreateAsync(CreateDepartmentRequest request, CancellationToken ct); Task UpdateAsync(int id, CreateDepartmentRequest request, CancellationToken ct); }
public interface IAttendanceService { Task<AttendanceDto> CheckInAsync(CheckInRequest request, CancellationToken ct); Task<AttendanceDto> CheckOutAsync(int id, CheckOutRequest request, CancellationToken ct); Task<PagedResult<AttendanceDto>> GetPagedAsync(int page, int pageSize, int? employeeId, CancellationToken ct); }
public interface ILeaveService { Task<int> CreateAsync(CreateLeaveRequest request, CancellationToken ct); Task<PagedResult<LeaveRequestDto>> GetPagedAsync(int page, int pageSize, LeaveRequestStatus? status, CancellationToken ct); Task DecideAsync(int id, LeaveDecisionRequest request, CancellationToken ct); }
public interface IOvertimeService { Task<int> CreateAsync(CreateOvertimeRequest request, CancellationToken ct); Task<IReadOnlyCollection<OvertimeDto>> GetPendingAsync(CancellationToken ct); Task DecideAsync(int id, bool approve, CancellationToken ct); }
public interface ILoanService { Task<int> CreateAsync(CreateLoanRequest request, CancellationToken ct); Task<IReadOnlyCollection<LoanDto>> GetPendingAsync(CancellationToken ct); Task DecideAsync(int id, bool approve, CancellationToken ct); }
public interface IPayrollService { Task<PayrollDto> GenerateAsync(int employeeId, int year, int month, CancellationToken ct); Task<IReadOnlyCollection<PayrollDto>> GetMonthAsync(int year, int month, CancellationToken ct); Task PayAsync(int id, CancellationToken ct); }
public interface IDashboardService { Task<DashboardDto> GetAsync(int year, int month, CancellationToken ct); }
public interface IUserService { Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken ct); }
