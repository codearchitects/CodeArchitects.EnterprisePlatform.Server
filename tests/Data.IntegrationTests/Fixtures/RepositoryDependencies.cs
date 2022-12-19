using CodeArchitects.Platform.Data.Tracking;

namespace CodeArchitects.Platform.Data.Fixtures;

public record RepositoryDependencies(
  RepositoryImplementation Implementation,
  DbProvider Provider,
  ITrackingContext TrackingContext);
