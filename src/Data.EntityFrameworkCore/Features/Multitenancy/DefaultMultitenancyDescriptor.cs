using CodeArchitects.Platform.Data.Features.Multitenancy;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal class DefaultMultitenancyDescriptor : IMultitenancyDescriptor
{
  public DefaultMultitenancyDescriptor(Type tenantIdType, bool disableModificationFilters)
  {
    TenantIdType = tenantIdType;
    UsesModificationInterceptors = !disableModificationFilters;
  }

  public Type TenantIdType { get; }

  public Type? MultitenancyContextType => typeof(ProfileMultitenancyContext<>).MakeGenericType(TenantIdType);

  public Func<IServiceProvider, IMultitenancyContext>? MultitenancyContextImplementationFactory => null;

  public bool UsesModificationInterceptors { get; }
}
