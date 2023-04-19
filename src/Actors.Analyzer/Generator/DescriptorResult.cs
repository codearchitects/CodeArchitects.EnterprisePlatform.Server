using CodeArchitects.Platform.Actors.Analyzer.Utils;

namespace CodeArchitects.Platform.Actors.Analyzer.Generator;

internal readonly record struct DescriptorResult(RecordList<ActorDescriptor> Descriptors, RecordList<DiagnosticReference> Diagnostics)
{
  private static readonly DescriptorResult s_empty = new(
    Descriptors: new RecordList<ActorDescriptor>(Array.Empty<ActorDescriptor>()),
    Diagnostics: new RecordList<DiagnosticReference>(Array.Empty<DiagnosticReference>()));

  public static ref readonly DescriptorResult Empty => ref s_empty;
}
