using CodeArchitects.Platform.Data.Features.Concurrency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Concurrency;

using TProperty =
#if NET6_0_OR_GREATER
  IReadOnlyProperty
#else
  IProperty
#endif
  ;

internal class SaveChangesInterceptor : Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor
{
  private readonly IConcurrencyTokenProvider _tokenProvider;

  public SaveChangesInterceptor(IConcurrencyTokenProvider tokenProvider)
  {
    _tokenProvider = tokenProvider;
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

    if (eventData.Context is not { } context)
      return;

    foreach (EntityEntry entry in context.ChangeTracker.Entries())
    {
      if (entry.State is not EntityState.Modified)
        continue;

      if (!entry.Metadata.TryGetConcurrencyToken(out TProperty? concurrencyToken))
        continue;

      PropertyEntry tokenEntry = entry.Property(concurrencyToken.Name);
      tokenEntry.CurrentValue = _tokenProvider.CreateToken(concurrencyToken.ClrType, tokenEntry.CurrentValue);
    }
  }
}
