using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
      _context.ChangeTracker.TrackGraph(entity, _context, node =>
      {
        if (node.Entry.State != EntityState.Detached)
        {
          return false;
        }
        if (!ReferenceEquals(node.Entry.Entity, entity) && node.InboundNavigation is SkipNavigation navigation)
        {

        }

        node.Entry.State = EntityState.Added;

        return true;
      });
    }

    public virtual void Update(TEntity entity)
    {
      _context.ChangeTracker.TrackGraph(entity, _context, node =>
      {
        if (node.Entry.State != EntityState.Detached)
        {
          return false;
        }
        bool isRoot = ReferenceEquals(node.Entry.Entity, entity);
        if (!isRoot && node.InboundNavigation is SkipNavigation navigation)
        {
          DbSet<Dictionary<string, object>> joinSet = node.NodeState.Set<Dictionary<string, object>>(navigation.JoinEntityType.Name);
          string? entityForeignKeyName = null;
          string? sourceEntityForeignKeyName = null;
          object entityId = ((IEntity)node.Entry.Entity).Id;
          object sourceEntityId = ((IEntity)node.SourceEntry.Entity).Id;

          foreach (ForeignKey foreignKey in navigation.JoinEntityType.GetForeignKeys())
          {
            if (foreignKey.PrincipalEntityType.ClrType == node.SourceEntry.Metadata.ClrType)
            {
              sourceEntityForeignKeyName = foreignKey.Properties[0].Name;
            }
            else if (foreignKey.PrincipalEntityType.ClrType == node.Entry.Metadata.ClrType)
            {
              entityForeignKeyName = foreignKey.Properties[0].Name;
            }
          }
          HashSet<object> existingIds = joinSet
            .Where(x => x[sourceEntityForeignKeyName] == sourceEntityId)
            .Select(x => x[entityForeignKeyName])
            .ToHashSet();

          if (!existingIds.Contains(entityId))
          {
            joinSet.Add(new Dictionary<string, object>
            {
              [entityForeignKeyName] = entityId,
              [sourceEntityForeignKeyName] = sourceEntityId
            });
          }
          node.Entry.State = EntityState.Unchanged;

          return false;
        }
        else
        {
          if (isRoot || node.NodeState.Find(node.Entry.Metadata.ClrType, ((IEntity)node.Entry.Entity).Id) is not null)
          {
            node.Entry.State = EntityState.Modified;
          }
          else
          {
            node.Entry.State = EntityState.Added;
          }
        }

        return true;
      });
    }

    public virtual void Delete(TEntity entity)
    {
      Entities.Remove(entity);
    }
  }
}
