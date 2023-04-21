using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Materialization;
using CodeArchitects.Platform.Data.Tracking;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand> : IExecutor<TDbCommand>
  where TDbCommand : DbCommand
{
  private readonly ICommandBuilder<TDbCommand> _commandBuilder;
  private readonly IMaterializer _materializer;
  private readonly ICommandInterceptor<TDbCommand> _interceptor;
  private readonly ITrackingContext _trackingContext;

  public Executor(
    ICommandBuilder<TDbCommand> commandBuilder,
    IMaterializer materializer,
    ICommandInterceptorAggregator<TDbCommand> interceptor,
    ITrackingContext trackingContext)
  {
    _commandBuilder = commandBuilder;
    _materializer = materializer;
    _interceptor = interceptor;
    _trackingContext = trackingContext;
  }
}
