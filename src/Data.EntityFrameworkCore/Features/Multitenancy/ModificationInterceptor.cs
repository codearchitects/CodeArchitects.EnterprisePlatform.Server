using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.Features.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal class ModificationInterceptor : IModificationInterceptor
{
  private readonly IMultitenancyContext _context;
  private readonly Func<string> _generateParameterName;

  public ModificationInterceptor(IMultitenancyContext context)
  {
    _context = context;

    int counter = 0;
    _generateParameterName = () =>
    {
      return $"__ca__multitenancy_{counter++}";
    };
  }

  public bool ShouldApply => _context.ShouldFilter;

  public void Apply(IUpdateEntry entry, IList<IColumnModification> modifications)
  {
    if (entry.EntityState is EntityState.Added)
      return;

    if (!entry.EntityType.TryGetMultitenancyColumnMappings(out IEnumerable<IColumnMapping>? mappings))
      return;

    foreach (IColumnMapping mapping in mappings)
    {
      modifications.Add(new ColumnModification(new ColumnModificationParameters(
        entry: entry,
        property: mapping.Property,
        column: mapping.Column,
        generateParameterName: _generateParameterName,
        typeMapping: mapping.TypeMapping,
        valueIsRead: false,
        valueIsWrite: false,
        columnIsKey: false,
        columnIsCondition: true,
        sensitiveLoggingEnabled: true)));
    }
  }
}
