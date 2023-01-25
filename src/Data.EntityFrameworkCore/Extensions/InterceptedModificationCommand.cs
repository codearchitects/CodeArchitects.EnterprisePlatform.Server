using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class InterceptedModificationCommand : ModificationCommand
{
  private IReadOnlyList<IColumnModification>? _columnModifications;

  public InterceptedModificationCommand(in ModificationCommandParameters modificationCommandParameters)
    : base(modificationCommandParameters)
  {
  }

  public InterceptedModificationCommand(in NonTrackedModificationCommandParameters modificationCommandParameters)
    : base(modificationCommandParameters)
  {
  }

  public override IReadOnlyList<IColumnModification> ColumnModifications => _columnModifications ??= CreateColumnModifications();

  private IReadOnlyList<IColumnModification> CreateColumnModifications()
  {
    IReadOnlyList<IColumnModification> baseResult = base.ColumnModifications;
    IReadOnlyList<IUpdateEntry> entries = Entries;
    if (entries.Count == 0)
      return baseResult;

    DbContext context = entries[0].Context;
    IEnumerable<IModificationInterceptor> interceptors = context.GetInfrastructure().GetServices<IModificationInterceptor>();

    List<IColumnModification>? result = null;
    foreach (IModificationInterceptor interceptor in interceptors)
    {
      if (!interceptor.ShouldApply)
        continue;

      result ??= new(baseResult);

      foreach (IUpdateEntry entry in entries)
      {
        interceptor.Apply(entry, result);
      }
    }

    return result ?? baseResult;
  }
}
