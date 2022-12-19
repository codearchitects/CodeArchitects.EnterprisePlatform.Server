using CodeArchitects.Platform.CodeAnalysis;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

[Experimental]
public interface ICommandInterceptor<TDbCommand>
  where TDbCommand : DbCommand
{
  void OnCommandCreated(OperationType operation, TDbCommand command);

  void OnCommandBuilt(OperationType operation, TDbCommand command);
}
