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
  public class Repository<TEntity, TKey> : IIncludeableRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
  {
    private readonly DbContext _context;
    private readonly IQueryable<TEntity>? _queryable;

    private Repository(DbContext context, DbSet<TEntity> entities, Expression<Func<TEntity, object?>>[] paths)
    {
      _context = context;
      Entities = entities;
      IQueryable<TEntity> queryable = entities;
      foreach (Expression<Func<TEntity, object?>> path in paths)
      {
        queryable = queryable.Include(path);
      }
      _queryable = queryable;
    }

    public Repository(DbContext context)
    {
      _context = context;
      Entities = context.Set<TEntity>();
    }

    public Repository(DbContext context, DbSet<TEntity> entities)
    {
      _context = context;
      Entities = entities;
    }

    protected DbSet<TEntity> Entities { get; }

    public IRepository<TEntity, TKey> Include(params Expression<Func<TEntity, object?>>[] paths)
    {
      return new Repository<TEntity, TKey>(_context, Entities, paths); // TODO: Considerare object pooling
    }

    public IQueryable<TEntity> Query()
    {
      return _queryable ?? Entities;
    }

    public TEntity? Find(TKey id)
    {
      return _queryable is null ? Entities.Find(id) : _queryable.SingleOrDefault(x => x.Id.Equals(id));
    }

    public ValueTask<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
      return (_queryable is null
        ? Entities.FindAsync(new object[] { id }, cancellationToken)
        : new ValueTask<TEntity>(_queryable.SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken)))!;
    }

    public virtual void Add(TEntity entity)
    {
      _context.ChangeTracker.TrackGraph(entity, entity, HandleNodeOnAdd);

      static bool HandleNodeOnAdd(EntityEntryGraphNode<TEntity> node)
      {
        if (node.Entry.State != EntityState.Detached || node.InboundNavigation is SkipNavigation)
        {
          return false;
        }

        node.Entry.State = EntityState.Added;
        return true;
      }
    }

    public virtual void Update(TEntity entity)
    {
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

        if (node.InboundNavigation is SkipNavigation)
        {
          return false;
        }

        if (node.Entry.Entity is IEntity entity)
        {
          object id = entity.Id;
          bool added =
             id is null                                  ||
            (id is Guid  guid   && guid   == Guid.Empty) ||
            (id is int   @int   && @int   == default)    ||
            (id is long  @long  && @long  == default)    ||
            (id is short @short && @short == default);

          node.Entry.State = added ? EntityState.Added : EntityState.Modified;
          return true;
        }
        else if (node.Entry.Entity is IAssociation)
        {
          node.Entry.State = EntityState.Added;
          return false;
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
