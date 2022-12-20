using CodeArchitects.Platform.Data.Features.SoftDelete;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

/// <summary>
/// Describes the soft delete feature of the CAEP extension.
/// </summary>
public interface ISoftDeleteDescriptor
{
  /// <summary>
  /// The implementation type of <see cref="ISoftDeleteContext"/>.
  /// </summary>
  Type? SoftDeleteContextType { get; }

  /// <summary>
  /// The implementation factory of <see cref="ISoftDeleteContext"/>.
  /// </summary>
  Func<IServiceProvider, ISoftDeleteContext>? SoftDeleteContextImplementationFactory { get; }
}
