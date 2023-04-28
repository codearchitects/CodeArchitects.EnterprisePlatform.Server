namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Specifies that a parameter must be resolved from the application service provider instead of the EFCore's internal service provider.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class ApplicationServiceAttribute : Attribute
{
}
