using Microsoft.EntityFrameworkCore.Update;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Intercepts modification commands.
/// </summary>
public interface IModificationInterceptor
{
  /// <summary>
  /// If <c>true</c> the interceptor will execute, if <c>false</c> the interceptor will not execute.
  /// </summary>
  bool ShouldApply { get; }

  /// <summary>
  /// Modifies a modification command.
  /// </summary>
  /// <param name="entry">The <see cref="IUpdateEntry"/> associated to the modification command.</param>
  /// <param name="modifications">The list of <see cref="IColumnModification"/>s associated to the modification command.</param>
  void Apply(IUpdateEntry entry, IList<IColumnModification> modifications);
}
