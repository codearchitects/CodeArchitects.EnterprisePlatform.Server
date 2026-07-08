using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.Features.SoftDelete;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

internal class ModificationInterceptor : IModificationInterceptor
{
  private readonly ISoftDeleteContext _context;
  private readonly Func<string> _generateParameterName;

  public ModificationInterceptor(ISoftDeleteContext context)
  {
    _context = context;

    int counter = 0;
    _generateParameterName = () =>
    {
      return $"__ca__softdelete_{counter++}";
    };
  }

  public bool ShouldApply => _context.ShouldFilter;

  public void Apply(IUpdateEntry entry, IList<IColumnModification> modifications)
  {
    if (!entry.EntityType.TryGetSoftDeleteColumnMappings(out IEnumerable<IColumnMapping>? mappings))
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