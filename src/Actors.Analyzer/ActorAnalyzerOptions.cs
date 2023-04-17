namespace CodeArchitects.Platform.Actors.Analyzer;

[Flags]
internal enum ActorAnalyzerOptions
{
  DisableActorDiagnostics = 1 << 0,
  DisableActorFactoryGeneration = 1 << 1,

  DisableAll = DisableActorDiagnostics | DisableActorFactoryGeneration
}
