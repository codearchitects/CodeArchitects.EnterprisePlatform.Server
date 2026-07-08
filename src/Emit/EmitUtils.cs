using System.Reflection;

namespace CodeArchitects.Platform.Emit;

internal static class EmitUtils
{
  public static readonly ConstructorInfo ObjectConstructor = typeof(object).GetRequiredConstructor();
}
