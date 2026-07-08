using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

internal interface ICommandInterceptorAggregator<TDbCommand> : ICommandInterceptor<TDbCommand>
  where TDbCommand : DbCommand
{
}
