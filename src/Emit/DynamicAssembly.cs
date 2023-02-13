using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit;

internal class DynamicAssembly
{
  public const string AssemblyName = "CodeArchitects.Platform.Dynamic";

  public static readonly AssemblyBuilder Assembly;
  public static readonly ModuleBuilder Module;

  static DynamicAssembly()
  {
    Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.RunAndCollect);
    Module = Assembly.DefineDynamicModule(AssemblyName);
  }

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
