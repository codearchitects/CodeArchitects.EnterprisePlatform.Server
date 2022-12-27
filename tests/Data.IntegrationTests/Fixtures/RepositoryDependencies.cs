namespace CodeArchitects.Platform.Data.Fixtures;

public record RepositoryDependencies(
  RepositoryImplementation Implementation,
  DbProvider Provider);
