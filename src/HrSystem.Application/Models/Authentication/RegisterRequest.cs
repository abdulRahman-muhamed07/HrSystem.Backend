namespace HrSystem.Application.Models.Authentication;

public sealed record RegisterRequest(string FullName, string Email, string Password);
