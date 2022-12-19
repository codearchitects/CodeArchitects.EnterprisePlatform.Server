using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures.Model;
using CodeArchitects.Platform.Data.Features.Multitenancy;
using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;

public class MultitenancyContext : IMultitenancyContext
{
  public readonly Guid Id = Guid.NewGuid();

  public bool ShouldFilter { get; set; } = true;

  public Guid TenantId { get; set; }

  object IMultitenancyContext.TenantId => TenantId;
}

public class MultitenancyDescriptor : IMultitenancyDescriptor
{
  private readonly MultitenancyContext _context;

  public MultitenancyDescriptor(MultitenancyContext context)
  {
    _context = context;
  }

  public bool UsesModificationInterceptors => true;

  public Type TenantIdType => typeof(Guid);

  public Type? MultitenancyContextType => null;

  public Func<IServiceProvider, IMultitenancyContext>? MultitenancyContextImplementationFactory => sp => _context;
}
