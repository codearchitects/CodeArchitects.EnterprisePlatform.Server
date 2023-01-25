using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;
using CodeArchitects.Platform.Data.Features.SoftDelete;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;

public class SoftDeleteContext : ISoftDeleteContext
{
  public bool ShouldFilter { get; set; } = true;
}

public class SoftDeleteDescriptor : ISoftDeleteDescriptor
{
  private readonly SoftDeleteContext _context;

  public SoftDeleteDescriptor(SoftDeleteContext context)
  {
    _context = context;
  }

  public Type? SoftDeleteContextType => null;

  public Func<IServiceProvider, ISoftDeleteContext>? SoftDeleteContextImplementationFactory => sp => _context;
}
