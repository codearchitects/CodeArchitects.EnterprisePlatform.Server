namespace CodeArchitects.Platform.Emit;

internal interface ILocalBuilder
{
  bool IsPinned { get; }
  int LocalIndex { get; }
  Type LocalType { get; }
}
