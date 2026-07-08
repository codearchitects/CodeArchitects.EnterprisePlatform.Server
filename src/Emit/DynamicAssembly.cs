using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit;

internal static class DynamicAssembly
{
  public const string AssemblyName = "CodeArchitects.Platform.Dynamic";

  public static readonly AssemblyBuilder Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.RunAndCollect);
  public static readonly ModuleBuilder Module = Assembly.DefineDynamicModule(AssemblyName);

  public static ModuleBuilder NewModule()
  {
    AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.RunAndCollect);
    return assembly.DefineDynamicModule(AssemblyName);
  }

  public static void IgnoreAccessCheckTo(string assemblyName)
  {
    Module.IgnoreAccessChecksTo(assemblyName);
  }
}
