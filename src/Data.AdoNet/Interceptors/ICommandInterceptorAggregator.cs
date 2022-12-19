using CodeArchitects.Platform.CodeAnalysis;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

[Experimental]
public interface ICommandInterceptorAggregator<TDbCommand> : ICommandInterceptor<TDbCommand>
  where TDbCommand : DbCommand
{
}
