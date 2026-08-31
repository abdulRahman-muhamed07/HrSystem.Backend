namespace HrSystem.Domain.Enums;

public enum UserRole { Employee, HR, Admin }
public enum EmploymentType { FullTime, PartTime, Contract, Probation }
public enum EmploymentStatus { Active, Resigned, Terminated, OnLeave }
public enum AttendanceStatus { OnTime, Late, Absent }
public enum LeaveRequestStatus { Pending, Approved, Rejected }
public enum LoanStatus { Pending, Approved, Rejected, Completed }
public enum OvertimeStatus { Pending, Approved, Rejected }
public enum PayrollStatus { Draft, Paid }
