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
}
