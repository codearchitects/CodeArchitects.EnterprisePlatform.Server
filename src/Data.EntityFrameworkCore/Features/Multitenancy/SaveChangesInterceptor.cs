using CodeArchitects.Platform.Data.Features.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal class SaveChangesInterceptor : Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor
{
  private readonly IMultitenancyContext _context;

  public SaveChangesInterceptor(IMultitenancyContext context)
  {
    _context = context;
  }

  public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
  {
    SavingChangesCore(eventData, result);

    return result;
  }

  public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
  {
    SavingChangesCore(eventData, result);

    return new(result);
  }

  private void SavingChangesCore(DbContextEventData eventData, InterceptionResult<int> result)
  {
    if (result.HasResult)
      return;

    if (!_context.ShouldFilter)
      return;

    if (eventData.Context is not { } context)
      return;

    foreach (EntityEntry entry in context.ChangeTracker.Entries())
    {
      if (!entry.Metadata.TryGetTenantIdPropertyName(out string? tenantIdPropertyName))
        continue;

      PropertyEntry tenantIdEntry = entry.Property(tenantIdPropertyName);
      object tenantId = _context.TenantId;
      switch (entry.State)
      {
        case EntityState.Added:
          tenantIdEntry.CurrentValue = tenantId;
          break;
        case EntityState.Modified or EntityState.Deleted:
          tenantIdEntry.OriginalValue = tenantId;
          tenantIdEntry.CurrentValue = tenantId;
          tenantIdEntry.IsModified = false;
          break;
      }
    }
  }
}
