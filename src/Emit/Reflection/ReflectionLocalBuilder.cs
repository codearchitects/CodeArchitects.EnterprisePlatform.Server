using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Reflection;

internal class ReflectionLocalBuilder : ILocalBuilder
{
  public ReflectionLocalBuilder(LocalBuilder builder)
  {
    Builder = builder;
  }

  public LocalBuilder Builder { get; }

  public bool IsPinned => Builder.IsPinned;

  public int LocalIndex => Builder.LocalIndex;

  public Type LocalType => Builder.LocalType;
}
