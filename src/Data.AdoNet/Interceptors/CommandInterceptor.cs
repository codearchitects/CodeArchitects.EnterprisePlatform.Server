using CodeArchitects.Platform.CodeAnalysis;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

/// <summary>
/// Default implementation of <see cref="ICommandInterceptor{TDbCommand}"/> which defines empty methods that can be overriden.
/// </summary>
/// <typeparam name="TDbCommand">The command type.</typeparam>
[Experimental]
public class CommandInterceptor<TDbCommand> : ICommandInterceptor<TDbCommand>
  where TDbCommand : DbCommand
{
  public virtual void OnCommandBuilt(OperationType operation, TDbCommand command)
  {
  }

  public virtual void OnCommandCreated(OperationType operation, TDbCommand command)
  {
  }
}
