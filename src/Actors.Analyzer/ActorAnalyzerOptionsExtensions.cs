namespace CodeArchitects.Platform.Actors.Analyzer;

internal static class ActorAnalyzerOptionsExtensions
{
  public static bool ShouldDisableActorDiagnostics(this ActorAnalyzerOptions options)
  {
    return (options & ActorAnalyzerOptions.DisableActorDiagnostics) != 0;
  }

  public static bool ShouldDisableActorFactoryGeneration(this ActorAnalyzerOptions options)
  {
    return (options & ActorAnalyzerOptions.DisableActorFactoryGeneration) != 0;
  }

  public static bool ShouldTreatNullableAsError(this ActorAnalyzerOptions options)
  {
    return (options & ActorAnalyzerOptions.TreatNullableAsError) != 0;
  }
}
