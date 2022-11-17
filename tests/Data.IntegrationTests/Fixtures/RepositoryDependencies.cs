using CodeArchitects.Platform.Data.Tracking;

namespace CodeArchitects.Platform.Data.Fixtures;

public record RepositoryDependencies(
  RepositoryImplementation Implementation,
  DatabaseProvider Provider,
  ITrackingContext TrackingContext);
