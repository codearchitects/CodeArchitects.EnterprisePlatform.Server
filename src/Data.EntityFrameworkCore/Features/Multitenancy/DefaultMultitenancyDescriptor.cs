using CodeArchitects.Platform.Data.Features.Multitenancy;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

public class DefaultMultitenancyDescriptor<TTenantId> : IMultitenancyDescriptor
  where TTenantId : IEquatable<TTenantId>
{
  public DefaultMultitenancyDescriptor()
  {
  }

  public DefaultMultitenancyDescriptor(bool usesModificationInterceptors)
  {
    UsesModificationInterceptors = usesModificationInterceptors;
  }

  public virtual Type TenantIdType => typeof(TTenantId);

  public virtual Type? MultitenancyContextType => typeof(ProfileMultitenancyContext<TTenantId>);

  public virtual Func<IServiceProvider, IMultitenancyContext>? MultitenancyContextImplementationFactory => null;

  public virtual bool UsesModificationInterceptors { get; }
}
