using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

public abstract class AdoNetRepositoryBase<TEntity, TKey> : Repository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  protected override Data.IDataContext Context => ContextCore;

  private protected abstract IDataContext ContextCore { get; }
}

public class AdoNetRepository<TEntity, TKey> : AdoNetRepositoryBase<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public AdoNetRepository(IDataContext context)
  {
    Context = context;
  }

  protected new IDataContext Context { get; }

  protected IDbConnection Connection => Context.Connection;

  private protected override IDataContext ContextCore => Context;
}
