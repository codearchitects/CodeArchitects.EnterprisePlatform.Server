using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Associations;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Navigation;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Query;
using CodeArchitects.Platform.Data.Navigation;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

internal sealed class DataContext<TDbContext> : IDataContext<TDbContext>
  where TDbContext : DbContext
{
  private readonly IStateManager<TDbContext> _stateManager;
  private readonly ITrackingContext _trackingContext;
  private readonly IPredicateProvider _predicateProvider;
  private readonly IDefaultEntityFactory _defaultEntityFactory;

  public DataContext(IStateManager<TDbContext> stateManager, ITrackingContext trackingContext, IPredicateProvider predicateProvider, IDefaultEntityFactory defaultEntityFactory)
  {
    _stateManager = stateManager;
    _trackingContext = trackingContext;
    _predicateProvider = predicateProvider;
    _defaultEntityFactory = defaultEntityFactory;
  }

  public TDbContext DbContext => _stateManager.DbContext;

  DbContext IDataContext.DbContext => _stateManager.DbContext;

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    Expression<Func<TEntity, bool>> keyPredicate = _predicateProvider.GetFindPredicate<TEntity, TKey>(key);

    return DbContext.Set<TEntity>().FirstOrDefaultAsync(keyPredicate, cancellationToken)!;
  }

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    Expression<Func<TEntity, bool>> keyPredicate = _predicateProvider.GetFindPredicate<TEntity, TKey>(key);

    Includer<TEntity> includer = new(DbContext.Set<TEntity>());
    includeAction(includer);

    return includer.Queryable.FirstOrDefaultAsync(keyPredicate, cancellationToken)!;
  }

  public async Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    DbContext.ChangeTracker.TrackGraph(entity, null, static delegate (EntityEntryGraphNode<object?> node)
    {
      switch (node.InboundNavigation)
      {
        case null: // Root entity
          node.Entry.State = EntityState.Added;
          return true;

        case ISkipNavigation skipNavigation: // Many-to-many navigation
          if (node.Entry.State is EntityState.Detached)
          {
            node.Entry.State = EntityState.Unchanged;
          }
          return false;

        case INavigation navigation: // One-to-one or many-to-many navigation
          if (navigation.IsOnDependent)
            return false;

          if (navigation.IsIntraAggregate())
          {
            node.Entry.State = EntityState.Added;
            return true;
          }
          else
          {
            foreach (IProperty property in navigation.ForeignKey.Properties)
            {
              node.Entry.Property(property.Name).IsModified = true;
            }
            return false;
          }

        default:
          Debug.Fail("This point should be unreachable.");
          return false;
      }
    });

    await _stateManager.SaveAsync(cancellationToken);
  }

  public async Task UpdateAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (_trackingContext.GetTrackingState(entity) is not TrackingState.Modified)
    {
      DbContext.Entry(entity).State = EntityState.Modified;
      await _stateManager.SaveAsync(cancellationToken);
    }

    List<Action> manyToManyUpdates = new();
    DbContext.ChangeTracker.TrackGraph(entity, null, delegate (EntityEntryGraphNode<object?> node)
    {
      TrackingState state = _trackingContext.GetTrackingState(node.Entry.Entity);
      switch (node.InboundNavigation)
      {
        case null:
          return true;
        case ISkipNavigation skipNavigation:
          switch (state)
          {
            case TrackingState.Added:
              var collectionAccessor = skipNavigation.GetCollectionAccessor()!;
              collectionAccessor.Remove(node.SourceEntry!.Entity, node.Entry.Entity);
              manyToManyUpdates.Add(() =>
              {
                collectionAccessor.Add(node.SourceEntry.Entity, node.Entry.Entity, false);
                node.Entry.State = EntityState.Unchanged;
              });
              break;
            case TrackingState.Removed:
              manyToManyUpdates.Add(() =>
              {
                skipNavigation.GetCollectionAccessor()!.Remove(node.SourceEntry!.Entity, node.Entry.Entity);
                node.Entry.State = EntityState.Unchanged;
              });
              break;
            case TrackingState.Detached or TrackingState.Unchanged:
              break;
            default:
              throw new InvalidTrackingStateException(node.Entry.Metadata.ClrType.Name, state);
          }
          return false;
        case INavigation navigation:
          return !navigation.IsOnDependent && navigation.IsIntraAggregate();
        default:
          Debug.Fail("This point should be unreachable.");
          return false;
      }
    });

    DbContext.Attach(entity); // TODO: Attach in the previous graph tracking

    DbContext.ChangeTracker.TrackGraph(entity, _trackingContext, static delegate (EntityEntryGraphNode<ITrackingContext> node)
    {
      ITrackingContext trackingContext = node.NodeState;
      TrackingState state = trackingContext.GetTrackingState(node.Entry.Entity);

      switch (node.InboundNavigation)
      {
        case null: // Root entity
          node.Entry.State = EntityState.Modified;
          return true;

        case ISkipNavigation skipNavigation: // Many-to-many navigation
          return false;

        case INavigation navigation: // One-to-one or one-to-many navigation
          if (navigation.IsOnDependent)
          {
            if (navigation.IsIntraAggregate())
              return false;

            switch (state)
            {
              case TrackingState.Added:
                foreach (IProperty property in navigation.ForeignKey.Properties)
                {
                  node.SourceEntry!.Property(property.Name).IsModified = true;
                }
                break;
              case TrackingState.Detached:
                break;
              default:
                throw new InvalidTrackingStateException(node.Entry.Metadata.ClrType.Name, state);
            }
            return false;
          }

          if (navigation.IsInterAggregate())
          {
            if (navigation.IsCollection)
            {
              switch (state)
              {
                case TrackingState.Added:
                  foreach (IProperty property in navigation.ForeignKey.Properties)
                  {
                    node.Entry.Property(property.Name).IsModified = true;
                  }
                  break;
                case TrackingState.Removed:
                  navigation.GetCollectionAccessor()!.Remove(node.SourceEntry!.Entity, node.Entry.Entity);
                  break;
                case TrackingState.Detached or TrackingState.Unchanged:
                  break;
                default:
                  throw new InvalidTrackingStateException(node.Entry.Metadata.ClrType.Name, state);
              }
            }
            else
            {
              switch (state)
              {
                case TrackingState.Added:
                  foreach (IProperty property in navigation.ForeignKey.Properties)
                  {
                    node.Entry.Property(property.Name).IsModified = true;
                  }
                  break;
                case TrackingState.Removed:
                  node.SourceEntry!.Navigation(navigation.Name).CurrentValue = null;
                  break;
                case TrackingState.Detached or TrackingState.Unchanged:
                  break;
                default:
                  throw new InvalidTrackingStateException(node.Entry.Metadata.ClrType.Name, state);
              }
            }
            return false;
          }

          switch (state)
          {
            case TrackingState.Added:
              node.Entry.State = EntityState.Added;
              break;
            case TrackingState.Removed:
              node.Entry.State = EntityState.Deleted;
              break;
            case TrackingState.Modified:
              node.Entry.State = EntityState.Modified;
              break;
            case TrackingState.Detached or TrackingState.Unchanged:
              break;
            default:
              throw new InvalidTrackingStateException(node.Entry.Metadata.ClrType.Name, state);
          }
          return true;

        default:
          Debug.Fail("This point should be unreachable.");
          return false;
      }
    });

    foreach (Action manyToManyUpdate in manyToManyUpdates)
    {
      manyToManyUpdate();
    }

    await _stateManager.SaveAsync(cancellationToken);
  }

  public async Task UpsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (await DbContext.Set<TEntity>().AnyAsync(_predicateProvider.GetFindPredicate<TEntity, TKey>(entity), cancellationToken))
    {
      await UpdateAsync<TEntity, TKey>(entity, cancellationToken);
    }
    else
    {
      await InsertAsync<TEntity, TKey>(entity, cancellationToken);
    }
  }

  public async Task RemoveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    DbContext.Set<TEntity>().Remove(entity);

    await _stateManager.SaveAsync(cancellationToken);
  }

  public async Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!_defaultEntityFactory.TryCreate(key, out TEntity? entity))
    {
      // TODO: Log warning
      entity = await FindAsync<TEntity, TKey>(key, cancellationToken)
        ?? throw new DbUpdateConcurrencyException("Entity was not found on the database."); // TODO: Improve message
    }

    DbContext.Set<TEntity>().Remove(entity);

    await _stateManager.SaveAsync(cancellationToken);
  }
}
