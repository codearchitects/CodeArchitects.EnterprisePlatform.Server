using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

/// <summary>
/// Indicates which particular <see cref="DbContext"/> must be used for seeding database.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SeedingContextAttribute : Attribute
{
  /// <summary>
  /// Creates a new instance of the <see cref="SeedingContextAttribute"/> class.
  /// </summary>
  /// <param name="dbContextType">The <see cref="DbContext"/> type to use for seeding the database.</param>
  public SeedingContextAttribute(Type dbContextType)
  {
    if (dbContextType is null)
      throw new ArgumentNullException(nameof(dbContextType));

    if (!typeof(DbContext).IsAssignableFrom(dbContextType))
      throw new ArgumentException($"'{dbContextType.Name}' does not extends '{nameof(DbContext)}'.", nameof(dbContextType));

    DbContextType = dbContextType;
  }

  /// <summary>
  /// The <see cref="DbContext"/> type to use for seeding the database.
  /// </summary>
  public Type DbContextType { get; }
}
