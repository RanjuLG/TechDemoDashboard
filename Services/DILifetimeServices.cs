namespace Demo_C_.Services;

/// <summary>
/// Transient service - A new instance is created every time it is requested.
/// The GUID will be different for every injection.
/// </summary>
public class TransientService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.Now;
}

/// <summary>
/// Scoped service - One instance per HTTP request/scope.
/// The GUID will be the same within a single request, but different across requests.
/// </summary>
public class ScopedService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.Now;
}

/// <summary>
/// Singleton service - One instance for the entire application lifetime.
/// The GUID will never change, even across page refreshes.
/// </summary>
public class SingletonService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.Now;
}
