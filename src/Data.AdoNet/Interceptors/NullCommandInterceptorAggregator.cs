using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

internal class NullCommandInterceptorAggregator<TDbCommand> : ICommandInterceptorAggregator<TDbCommand>
  where TDbCommand : DbCommand
{
  public void OnCommandCreated(OperationType operation, TDbCommand command)
  {
    // No-op, since there are no interceptors to aggregate
  }

  public void OnCommandBuilt(OperationType operation, TDbCommand command)
  {
    // No-op, since there are no interceptors to aggregate
  }
}
