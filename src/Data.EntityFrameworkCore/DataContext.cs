using CodeArchitects.Platform.Common.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

/// <summary>
/// Base data context class, for configuring multi-tenancy and entity registration.
/// </summary>
public class DataContext : DataContextBase
{
  public DataContext()
  {
  }

  public DataContext(DbContextOptions options)
    : base(options)
  {
  }

  public DataContext(IIdentityProfile identity)
    : base(identity)
  {
  }

  public DataContext(IIdentityProfile identity, DbContextOptions options)
    : base(identity, options)
  {
  }

  /// <inheritdoc cref="DbContext.OnModelCreating(ModelBuilder)"/>
  protected new virtual void OnModelCreating(ModelBuilder modelBuilder)
  {
  }

  protected sealed override void OnModelCreatingCore(ModelBuilder modelBuilder)
  {
    OnModelCreating(modelBuilder);
  }
}

/// <summary>
/// Base class for <see cref="DataContext"/>.
/// </summary>
/// <remarks>
/// This class exists to address the drawback of overriding <see cref="OnModelCreating(ModelBuilder)"/> in a derived class,
/// which would be otherwise forced to call <c>base.OnModelCreating(modelBuilder)</c>.
/// </remarks>
public abstract class DataContextBase : DbContext
{
  private static readonly MethodInfo s_getTenantIdFilterMethod;

  static DataContextBase()
  {
    s_getTenantIdFilterMethod = typeof(DataContextBase).GetMethod(
      name: nameof(GetTenantIdFilter),
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
      binder: null,
      types: Type.EmptyTypes,
      modifiers: null) ?? throw new MissingMethodException(typeof(DataContextBase).Name, nameof(GetTenantIdFilter));
  }

  private readonly Guid _tenantId;

  public DataContextBase()
  {
  }

  public DataContextBase(DbContextOptions options)
    : base(options)
  {
  }

  public DataContextBase(IIdentityProfile identity)
  {
    _tenantId = identity.IsAuthenticated ? identity.TenantId : Guid.Empty;
  }

  public DataContextBase(IIdentityProfile identity, DbContextOptions options)
    : base(options)
  {
    _tenantId = identity.IsAuthenticated ? identity.TenantId : Guid.Empty;
  }

  public override EntityEntry Attach(object entity)
  {
    SetTenantId();
    return base.Attach(entity);
  }

  public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
  {
    SetTenantId();
    return base.Attach(entity);
  }

  public override void AttachRange(IEnumerable<object> entities)
  {
    SetTenantId();
    base.AttachRange(entities);
  }

  public override void AttachRange(params object[] entities)
  {
    SetTenantId();
    base.AttachRange(entities);
  }

  public override EntityEntry Remove(object entity)
  {
    SetTenantId();
    return base.Remove(entity);
  }

  public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
  {
    SetTenantId();
    return base.Remove(entity);
  }

  public override void RemoveRange(IEnumerable<object> entities)
  {
    SetTenantId();
    base.RemoveRange(entities);
  }

  public override void RemoveRange(params object[] entities)
  {
    SetTenantId();
    base.RemoveRange(entities);
  }

  public override EntityEntry Update(object entity)
  {
    SetTenantId();
    return base.Update(entity);
  }

  public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
  {
    SetTenantId();
    return base.Update(entity);
  }

  public override void UpdateRange(IEnumerable<object> entities)
  {
    SetTenantId();
    base.UpdateRange(entities);
  }

  public override void UpdateRange(params object[] entities)
  {
    SetTenantId();
    base.UpdateRange(entities);
  }

  public override int SaveChanges(bool acceptAllChangesOnSuccess)
  {
    SetTenantId();
    return base.SaveChanges(acceptAllChangesOnSuccess);
  }

  public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
  {
    SetTenantId();
    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
  }

  internal int SaveChangesWithoutTenantId()
  {
    return base.SaveChanges(true);
  }

  protected abstract void OnModelCreatingCore(ModelBuilder modelBuilder);

  protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
  {
    OnModelCreatingCore(modelBuilder);
    ApplyTenantEntityFilters(modelBuilder.Model);
  }

  private void ApplyTenantEntityFilters(IMutableModel model)
  {
    IEnumerable<IMutableEntityType> tenantEntityTypes = model
      .GetEntityTypes()
      .Where(entityType => entityType.ClrType is not null && typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType));

    foreach (IMutableEntityType tenantEntityType in tenantEntityTypes)
    {
      LambdaExpression filter = (LambdaExpression)s_getTenantIdFilterMethod.MakeGenericMethod(tenantEntityType.ClrType).Invoke(this, Array.Empty<object>());
      tenantEntityType.SetQueryFilter(filter);
    }
  }

  private Expression<Func<TTenantEntity, bool>> GetTenantIdFilter<TTenantEntity>()
    where TTenantEntity : ITenantEntity
  {
    return entity => entity.TenantId == _tenantId;
  }

  private void SetTenantId()
  {
    foreach (var entry in ChangeTracker.Entries())
    {
      if (entry.State is not EntityState.Added and not EntityState.Modified and not EntityState.Deleted)
        continue;

      if (entry.Entity is not ITenantEntity tenantEntity)
        continue;

      if (tenantEntity.TenantId is null)
      {
        tenantEntity.TenantId = _tenantId;
      }
      else if (tenantEntity.TenantId != _tenantId)
      {
        throw new InvalidOperationException($"Invalid tenant id for entity of type '{tenantEntity.GetType()}'.");
      }
    }
  }
}
