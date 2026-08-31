namespace HrSystem.Application.Exceptions;

public sealed class ConcurrencyConflictException() : Exception("The resource was modified by another request.");
