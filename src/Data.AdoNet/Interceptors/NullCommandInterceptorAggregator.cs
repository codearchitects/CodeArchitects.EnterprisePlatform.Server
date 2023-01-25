using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

internal class NullCommandInterceptorAggregator<TDbCommand> : ICommandInterceptorAggregator<TDbCommand>
  where TDbCommand : DbCommand
{
  public void OnCommandCreated(OperationType operation, TDbCommand command)
  {
  }

  public void OnCommandBuilt(OperationType operation, TDbCommand command)
  {
  }
}
