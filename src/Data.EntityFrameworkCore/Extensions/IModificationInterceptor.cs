using Microsoft.EntityFrameworkCore.Update;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

public interface IModificationInterceptor
{
  bool ShouldApply { get; }
  void Apply(IUpdateEntry entry, IList<IColumnModification> modifications);
}
