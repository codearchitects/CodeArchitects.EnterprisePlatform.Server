using CodeArchitects.Platform.Data.Features.SoftDelete;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

/// <summary>
/// Default implementation of <see cref="ISoftDeleteDescriptor"/>.
/// </summary>
public class DefaultSoftDeleteDescriptor : ISoftDeleteDescriptor
{
  /// <inheritdoc/>
  public virtual Type? SoftDeleteContextType => typeof(DefaultSoftDeleteContext);

  /// <inheritdoc/>
  public virtual Func<IServiceProvider, ISoftDeleteContext>? SoftDeleteContextImplementationFactory => null;
}
