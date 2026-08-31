namespace HrSystem.Domain;

public interface IConcurrencyTracked
{
    Guid Version { get; }
}
