using CodeArchitects.Platform.Data.Features.SoftDelete;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

/// <summary>
/// Default implementation of <see cref="ISoftDeleteDescriptor"/>.
/// </summary>
public class DefaultSoftDeleteDescriptor : ISoftDeleteDescriptor
{
  public virtual Type? SoftDeleteContextType => typeof(DefaultSoftDeleteContext);

  public virtual Func<IServiceProvider, ISoftDeleteContext>? SoftDeleteContextImplementationFactory => null;
}
