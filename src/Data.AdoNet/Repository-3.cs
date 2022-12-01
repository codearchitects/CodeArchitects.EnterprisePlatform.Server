using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

public class Repository<TConnection, TEntity, TKey> : Repository<TEntity, TKey>
  where TConnection : DbConnection
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public Repository(IDataContext<TConnection> context)
    : base(context)
  {
    Context = context;
  }

  protected new IDataContext<TConnection> Context { get; }

  protected new TConnection Connection => Context.Connection;
}
