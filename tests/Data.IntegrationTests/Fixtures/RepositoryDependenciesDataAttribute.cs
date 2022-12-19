using CodeArchitects.Platform.Data.Tracking;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Data.Fixtures;

public class RepositoryDependenciesDataAttribute : DataAttribute
{
  public override IEnumerable<object[]> GetData(MethodInfo testMethod)
  {
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.AdoNet, DbProvider.SqlServer, new TrackingContext()) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.AdoNet, DbProvider.Postgres, new TrackingContext()) };
    // yield return new object[] { new RepositoryDependencies(RepositoryImplementation.AdoNet, DbProvider.Oracle, new TrackingContext()) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.EFCore, DbProvider.SqlServer, new TrackingContext()) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.EFCore, DbProvider.Postgres, new TrackingContext()) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.EFCore, DbProvider.Oracle, new TrackingContext()) };
  }
}
