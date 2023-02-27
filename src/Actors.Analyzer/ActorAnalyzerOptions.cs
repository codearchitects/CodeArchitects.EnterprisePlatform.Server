namespace CodeArchitects.Platform.Actors.Analyzer;

[Flags]
internal enum ActorAnalyzerOptions
{
  None = 0,
  DisableDiagnostics = 1,
  DisableFactoryGeneration = 2
}
