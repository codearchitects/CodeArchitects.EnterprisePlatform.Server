using CodeArchitects.Platform.CodeAnalysis;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

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
