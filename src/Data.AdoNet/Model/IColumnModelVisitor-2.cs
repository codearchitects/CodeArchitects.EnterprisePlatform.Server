using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents an operation to be performed on a column model.
/// </summary>
/// <typeparam name="TResult">The type of the result of the operation.</typeparam>
/// <typeparam name="TState">The type of the state passed to the visitor.</typeparam>
[Experimental]
public interface IColumnModelVisitor<out TResult, TState>
{
  /// <summary>
  /// Visits an <see cref="IForeignKeyColumnModel"/>.
  /// </summary>
  /// <param name="columnModel">The column model.</param>
  /// <param name="state">The state passed to the method.</param>
  /// <returns>The result of the operation.</returns>
  TResult VisitForeignKey(IForeignKeyColumnModel columnModel, in TState state);

  /// <summary>
  /// Visits an <see cref="IOrdinaryColumnModel"/>.
  /// </summary>
  /// <param name="columnModel">The column model.</param>
  /// <param name="state">The state passed to the method.</param>
  /// <returns>The result of the operation.</returns>
  TResult VisitOrdinary(IOrdinaryColumnModel columnModel, in TState state);

  /// <summary>
  /// Visits an <see cref="IPrimaryAndForeignKeyColumnModel"/>.
  /// </summary>
  /// <param name="columnModel">The column model.</param>
  /// <param name="state">The state passed to the method.</param>
  /// <returns>The result of the operation.</returns>
  TResult VisitPrimaryAndForeignKey(IPrimaryAndForeignKeyColumnModel columnModel, in TState state);

  /// <summary>
  /// Visits an <see cref="IPrimaryKeyColumnModel"/>.
  /// </summary>
  /// <param name="columnModel">The column model.</param>
  /// <param name="state">The state passed to the method.</param>
  /// <returns>The result of the operation.</returns>
  TResult VisitPrimaryKey(IPrimaryKeyColumnModel columnModel, in TState state);
}
