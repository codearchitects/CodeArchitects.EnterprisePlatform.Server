using CodeArchitects.Platform.Data.Features.Multitenancy;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

public interface IMultitenancyDescriptor
{
  bool UsesModificationInterceptors { get; }
  Type TenantIdType { get; }
  Type? MultitenancyContextType { get; }
  Func<IServiceProvider, IMultitenancyContext>? MultitenancyContextImplementationFactory { get; }
}
