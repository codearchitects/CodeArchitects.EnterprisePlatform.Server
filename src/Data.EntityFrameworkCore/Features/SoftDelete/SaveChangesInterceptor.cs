using CodeArchitects.Platform.Data.Features.SoftDelete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

internal class SaveChangesInterceptor : Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor
{
  private readonly ISoftDeleteContext _context;

  public SaveChangesInterceptor(ISoftDeleteContext context)
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
      if (!entry.Metadata.TryGetSoftDeletePropertyName(out string? propertyName))
        continue;

      PropertyEntry flagEntry = entry.Property(propertyName);
      switch (entry.State)
      {
        case EntityState.Added:
          flagEntry.CurrentValue = false;
          break;
        case EntityState.Modified:
          flagEntry.CurrentValue = false;
          flagEntry.IsModified = false;
          break;
        case EntityState.Deleted:
          entry.State = EntityState.Modified;
          foreach (PropertyEntry propertyEntry in entry.Properties)
          {
            propertyEntry.IsModified = false;
          }
          flagEntry.CurrentValue = true;
          flagEntry.IsModified = true;
          break;
      }
    }
  }
}
