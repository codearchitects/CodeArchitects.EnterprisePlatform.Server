using CodeArchitects.Platform.Data.Tracking;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Data.Fixtures;

public class RepositoryDependenciesDataAttribute : DataAttribute
{
  public override IEnumerable<object[]> GetData(MethodInfo testMethod)
  {
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.AdoNet, DatabaseProvider.SqlServer, new TrackingContext()) };
    // yield return new object[] { new RepositoryDependencies(RepositoryImplementation.AdoNet, DatabaseProvider.Postgres, new TrackingContext()) };
    // yield return new object[] { new RepositoryDependencies(RepositoryImplementation.EntityFrameworkCore, DatabaseProvider.SqlServer, new TrackingContext()) };
    // yield return new object[] { new RepositoryDependencies(RepositoryImplementation.EntityFrameworkCore, DatabaseProvider.Postgres, new TrackingContext()) };
  }
}
