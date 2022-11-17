namespace CodeArchitects.Platform.Data.Features.Multitenancy;

public interface IMultitenancyContext
{
  bool ShouldFilter { get; }
  object TenantId { get; }
}
