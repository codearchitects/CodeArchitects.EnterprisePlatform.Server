using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.Fixtures;

public sealed class TestScope : IDisposable
{
  private readonly IServiceScope _scope;
  private readonly RepositoryImplementation _implementation;

  public TestScope(IServiceScope scope, RepositoryImplementation implementation)
  {
    _scope = scope;
    _implementation = implementation;
  }

  public IServiceProvider Services => _scope.ServiceProvider;

  public TestDbContext DbContext => Services.GetRequiredService<TestDbContext>();

  public ITrackingContext TrackingContext => Services.GetRequiredService<ITrackingContext>();

  public Repository<TEntity, TKey> CreateRepository<TEntity, TKey>()
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _implementation switch
    {
      RepositoryImplementation.AdoNet => new AdoNetRepository<TEntity, TKey>(Services.GetRequiredService<AdoNet.IDataContext>()),
      RepositoryImplementation.EFCore => new EFCoreRepository<TEntity, TKey>(Services.GetRequiredService<EntityFrameworkCore.IDataContext>()),
      _                               => throw Errors.Unreachable
    };
  }

  public static TestScope Create(IServiceProvider services, RepositoryImplementation implementation)
  {
    return new(services.CreateScope(), implementation);
  }

  public void Dispose()
  {
    _scope.Dispose();
  }
}
