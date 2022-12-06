using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

public abstract class AdoNetRepositoryBase<TEntity, TKey> : Repository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  protected override IDataContext Context => ContextCore;

  private protected abstract IAdoNetContext ContextCore { get; }
}

public class AdoNetRepository<TEntity, TKey> : AdoNetRepositoryBase<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public AdoNetRepository(IAdoNetContext context)
  {
    Context = context;
  }

  protected new IAdoNetContext Context { get; }

  protected IDbConnection Connection => Context.Connection;

  private protected override IAdoNetContext ContextCore => Context;
}
