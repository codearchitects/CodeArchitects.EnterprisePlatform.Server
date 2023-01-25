using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

internal class CommandInterceptorAggregator<TDbCommand> : ICommandInterceptorAggregator<TDbCommand>
  where TDbCommand : DbCommand
{
  private readonly IEnumerable<ICommandInterceptor<TDbCommand>> _interceptors;

  public CommandInterceptorAggregator(IEnumerable<ICommandInterceptor<TDbCommand>> interceptors)
  {
    _interceptors = interceptors;
  }

  public void OnCommandCreated(OperationType operation, TDbCommand command)
  {
    foreach (var interceptor in _interceptors)
    {
      interceptor.OnCommandCreated(operation, command);
    }
  }

  public void OnCommandBuilt(OperationType operation, TDbCommand command)
  {
    foreach (var interceptor in _interceptors)
    {
      interceptor.OnCommandBuilt(operation, command);
    }
  }
}
