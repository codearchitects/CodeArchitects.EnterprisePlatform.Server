using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Data.Fixtures;

public class RepositoryDependenciesDataAttribute : DataAttribute
{
  public override IEnumerable<object[]> GetData(MethodInfo testMethod)
  {
    foreach (var dependency in GetData(async: true))
    {
      yield return new[] { dependency };
    }
    foreach (var dependency in GetData(async: false))
    {
      yield return new[] { dependency };
    }
  }

  private IEnumerable<RepositoryDependencies> GetData(bool async)
  {
    yield return new RepositoryDependencies(DataImplementation.AdoNet, DbProvider.SqlServer, async);
    yield return new RepositoryDependencies(DataImplementation.AdoNet, DbProvider.Postgres, async);
    // yield return new RepositoryDependencies(RepositoryImplementation.AdoNet, DbProvider.Oracle, async);
    yield return new RepositoryDependencies(DataImplementation.AdoNet, DbProvider.MariaDb, async);
    yield return new RepositoryDependencies(DataImplementation.EFCore, DbProvider.SqlServer, async);
    yield return new RepositoryDependencies(DataImplementation.EFCore, DbProvider.Postgres, async);
    yield return new RepositoryDependencies(DataImplementation.EFCore, DbProvider.Oracle, async);
    yield return new RepositoryDependencies(DataImplementation.EFCore, DbProvider.MariaDb, async);
  }
}
