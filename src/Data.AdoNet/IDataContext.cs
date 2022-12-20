using CodeArchitects.Platform.CodeAnalysis;
using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// Represents a <see cref="Data.IDataContext"/> that is based on ADO.NET.
/// </summary>
public interface IDataContext : Data.IDataContext
{
  /// <summary>
  /// The connection used by this data context.
  /// </summary>
  IDbConnection Connection { get; }

  /// <summary>
  /// The database model.
  /// </summary>
  IDataModel Model { get; }

  /// <summary>
  /// Executes an operation within the scope of a unit of work. Delays the execution of the specified operation until the unit of work completes.
  /// </summary>
  /// <param name="execution">The operation that will be executed on the unit of work completion.</param>
  /// <param name="startTransaction">A value indicating whether a new transaction should be started before executing the delayed operations.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task BatchExecuteAsync(Execution<IDbConnection, IDbTransaction> execution, bool startTransaction, CancellationToken cancellationToken = default);

  /// <summary>
  /// Visits all nodes in the graph represented by the specified entity, except the root node.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TState">The type of the state object passed to the callback.</typeparam>
  /// <param name="entity">The root entity of the graph to be visited.</param>
  /// <param name="state">The state object passed to the callback.</param>
  /// <param name="callback">The callback delegate that is called for each visited node.</param>
  [Experimental]
  void VisitGraph<TEntity, TState>(TEntity entity, TState state, VisitNodeCallback<TState> callback)
    where TEntity : class;

  /// <summary>
  /// Asynchronously visits all nodes in the graph represented by the specified entity, except the root node.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TState">The type of the state object passed to the callback.</typeparam>
  /// <param name="entity">The root entity of the graph to be visited.</param>
  /// <param name="state">The state object passed to the callback.</param>
  /// <param name="callback">The callback delegate that is called for each visited node.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  [Experimental]
  Task VisitGraphAsync<TEntity, TState>(TEntity entity, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
    where TEntity : class;
}
