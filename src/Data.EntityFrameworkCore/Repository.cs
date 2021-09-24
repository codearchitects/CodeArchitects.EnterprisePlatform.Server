using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore
{
  public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
  {
    private readonly DbContext _context;

    public Repository(DbContext context)
      : this(context, context.Set<TEntity>())
    {
    }

    public Repository(DbContext context, DbSet<TEntity> entities)
    {
      _context = context;
      Entities = entities;
    }

    protected DbSet<TEntity> Entities { get; }

    public IQueryable<TEntity> Query(params Expression<Func<TEntity, object?>>[] paths)
    {
      IQueryable<TEntity> result = Entities;
      foreach (Expression<Func<TEntity, object?>> include in paths)
      {
        result = result.Include(include);
      }
      return result;
    }

    public virtual TEntity? Find(TKey id, params Expression<Func<TEntity, object?>>[] paths)
    {
      return paths.Length == 0
        ? Entities.Find(new object[] { id })
        : Query(paths).FirstOrDefault(x => x.Id.Equals(id));
    }

    public virtual ValueTask<TEntity?> FindAsync(TKey id, Expression<Func<TEntity, object?>>[]? paths = null, CancellationToken cancellationToken = default)
    {
      return (paths is null || paths.Length == 0
        ? Entities.FindAsync(new object[] { id }, cancellationToken)
        : new ValueTask<TEntity>(Query(paths).FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken)))!;
    }

    public virtual void Add(TEntity entity)
    {
      // TODO: Add ObjectState support
      _context.ChangeTracker.TrackGraph(entity, entity, HandleNodeOnAdd);

      static bool HandleNodeOnAdd(EntityEntryGraphNode<TEntity> node)
      {
        if (node.Entry.State != EntityState.Detached || node.InboundNavigation is SkipNavigation || node.InboundNavigation is Navigation navigation && navigation.IsOnDependent)
        {
          return false;
        }

        node.Entry.State = EntityState.Added;
        return true;
      }
    }

    public virtual void Update(TEntity entity)
    {
      // TODO: Add ObjectState support
      _context.ChangeTracker.TrackGraph(entity, entity, HandleNodeOnUpdate);

      static bool HandleNodeOnUpdate(EntityEntryGraphNode<TEntity> node)
      {
        if (node.Entry.State != EntityState.Detached)
        {
          return false;
        }
        if (ReferenceEquals(node.Entry.Entity, node.NodeState))
        {
          node.Entry.State = EntityState.Modified;
          return true;
        }

        if (node.InboundNavigation is SkipNavigation || node.InboundNavigation is Navigation navigation && navigation.IsOnDependent)
        {
          return false;
        }

        if (node.Entry.Entity is IEntity entity)
        {
          object id = entity.Id;
          bool added = // TODO: Temporary solution
             id is null                                  ||
            (id is Guid  guid   && guid   == Guid.Empty) ||
            (id is int   @int   && @int   == default)    ||
            (id is long  @long  && @long  == default)    ||
            (id is short @short && @short == default);

          node.Entry.State = added ? EntityState.Added : EntityState.Modified;
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    public virtual void Delete(TEntity entity)
    {
      Entities.Remove(entity);
    }
  }
}
