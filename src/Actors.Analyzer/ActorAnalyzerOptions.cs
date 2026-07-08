namespace CodeArchitects.Platform.Actors.Analyzer;

[Flags]
internal enum ActorAnalyzerOptions
{
  DisableActorDiagnostics = 1 << 0,
  DisableActorFactoryGeneration = 1 << 1,
  TreatNullableAsError = 1 << 2,

  DisableAll = DisableActorDiagnostics | DisableActorFactoryGeneration
}
