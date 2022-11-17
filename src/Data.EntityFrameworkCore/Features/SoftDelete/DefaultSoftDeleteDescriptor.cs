using CodeArchitects.Platform.Data.Features.SoftDelete;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

internal class DefaultSoftDeleteDescriptor : ISoftDeleteDescriptor
{
  public Type? SoftDeleteContextType => typeof(DefaultSoftDeleteContext);

  public Func<IServiceProvider, ISoftDeleteContext>? SoftDeleteContextImplementationFactory => null;
}
