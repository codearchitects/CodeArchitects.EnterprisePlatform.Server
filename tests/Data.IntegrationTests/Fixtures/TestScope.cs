using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.Fixtures;

public sealed class TestScope : IDisposable
{
  private readonly IServiceScope _scope;
  private readonly DataImplementation _implementation;
  private readonly bool _async;

  public TestScope(IServiceScope scope, DataImplementation implementation, bool async)
  {
    _scope = scope;
    _implementation = implementation;
    _async = async;
  }

  public IServiceProvider Services => _scope.ServiceProvider;

  public TestDbContext DbContext => Services.GetRequiredService<TestDbContext>();

  public ITrackingContext TrackingContext => Services.GetRequiredService<ITrackingContext>();

  public IRepository<TEntity, TKey> CreateRepository<TEntity, TKey>()
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IRepository<TEntity, TKey> repository = _implementation switch
    {
      DataImplementation.AdoNet => new AdoNetRepository<TEntity, TKey>(Services.GetRequiredService<AdoNet.IDataContext>()),
      DataImplementation.EFCore => new EFCoreRepository<TEntity, TKey>(Services.GetRequiredService<EntityFrameworkCore.IDataContext>()),
      _                         => throw Errors.Unreachable
    };

    return new TestRepository<TEntity, TKey>(repository, _async);
  }

  public static TestScope Create(IServiceProvider services, RepositoryDependencies dependencies)
  {
    return new(services.CreateScope(), dependencies.Implementation, dependencies.Async);
  }

  public void Dispose()
  {
    _scope.Dispose();
  }
}
