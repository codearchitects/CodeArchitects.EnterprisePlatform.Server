using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

public class AdoNetRepository<TConnection, TEntity, TKey> : AdoNetRepository<TEntity, TKey>
  where TConnection : DbConnection
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public AdoNetRepository(IAdoNetContext<TConnection> context)
    : base(context)
  {
  }

  protected new IAdoNetContext<TConnection> Context => (IAdoNetContext<TConnection>)base.Context;

  protected new TConnection Connection => Context.Connection;
}
