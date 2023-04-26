using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Data.Fixtures;

public class RepositoryDependenciesDataAttribute : DataAttribute
{
  public override IEnumerable<object[]> GetData(MethodInfo testMethod)
  {
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.AdoNet, DbProvider.SqlServer) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.AdoNet, DbProvider.Postgres) };
    //yield return new object[] { new RepositoryDependencies(RepositoryImplementation.AdoNet, DbProvider.Oracle) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.AdoNet, DbProvider.MariaDb) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.EFCore, DbProvider.SqlServer) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.EFCore, DbProvider.Postgres) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.EFCore, DbProvider.Oracle) };
    yield return new object[] { new RepositoryDependencies(RepositoryImplementation.EFCore, DbProvider.MariaDb) };
  }
}
