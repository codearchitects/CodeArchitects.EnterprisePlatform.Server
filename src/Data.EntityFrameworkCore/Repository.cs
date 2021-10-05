using CodeArchitects.Platform.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore
{
  [Experimental]
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

    public virtual TEntity? Find(TKey id)
    {
      return Entities.Find(new object[] { id });
    }

    public virtual ValueTask<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
      return Entities.FindAsync(new object[] { id }, cancellationToken)!;
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
