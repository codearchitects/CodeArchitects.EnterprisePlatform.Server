using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents an operation to be performed on a column model.
/// </summary>
/// <typeparam name="TResult">The type of the result of the operation.</typeparam>
[Experimental]
public interface IColumnModelVisitor<out TResult>
{
  /// <summary>
  /// Visits an <see cref="IForeignKeyColumnModel"/>.
  /// </summary>
  /// <param name="columnModel">The column model.</param>
  /// <returns>The result of the operation.</returns>
  TResult VisitForeignKey(IForeignKeyColumnModel columnModel);

  /// <summary>
  /// Visits an <see cref="IOrdinaryColumnModel"/>.
  /// </summary>
  /// <param name="columnModel">The column model.</param>
  /// <returns>The result of the operation.</returns>
  TResult VisitOrdinary(IOrdinaryColumnModel columnModel);

  /// <summary>
  /// Visits an <see cref="IPrimaryAndForeignKeyColumnModel"/>.
  /// </summary>
  /// <param name="columnModel">The column model.</param>
  /// <returns>The result of the operation.</returns>
  TResult VisitPrimaryAndForeignKey(IPrimaryAndForeignKeyColumnModel columnModel);

  /// <summary>
  /// Visits an <see cref="IPrimaryKeyColumnModel"/>.
  /// </summary>
  /// <param name="columnModel">The column model.</param>
  /// <returns>The result of the operation.</returns>
  TResult VisitPrimaryKey(IPrimaryKeyColumnModel columnModel);
}
