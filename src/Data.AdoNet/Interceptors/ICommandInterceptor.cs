using CodeArchitects.Platform.CodeAnalysis;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

/// <summary>
/// Allows interception of the command after it is created and after it is built.
/// </summary>
/// <typeparam name="TDbCommand">The command type.</typeparam>
[Experimental]
public interface ICommandInterceptor<TDbCommand>
  where TDbCommand : DbCommand
{
  /// <summary>
  /// Called immediately after the command is created.
  /// </summary>
  /// <param name="operation">The current operation type.</param>
  /// <param name="command">The command.</param>
  void OnCommandCreated(OperationType operation, TDbCommand command);

  /// <summary>
  /// Called immediately after the command is built.
  /// </summary>
  /// <param name="operation">The current operation type.</param>
  /// <param name="command">The command.</param>
  void OnCommandBuilt(OperationType operation, TDbCommand command);
}
