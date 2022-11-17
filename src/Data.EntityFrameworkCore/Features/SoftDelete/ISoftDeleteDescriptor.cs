using CodeArchitects.Platform.Data.Features.SoftDelete;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

public interface ISoftDeleteDescriptor
{
  Type? SoftDeleteContextType { get; }
  Func<IServiceProvider, ISoftDeleteContext>? SoftDeleteContextImplementationFactory { get; }
}
