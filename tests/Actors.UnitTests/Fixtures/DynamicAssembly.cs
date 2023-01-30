using CodeArchitects.Platform.Emit;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Fixtures;

internal static class DynamicAssembly
{
  public static ModuleBuilder CreateModule()
  {
    AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Emit.DynamicAssembly.AssemblyName), AssemblyBuilderAccess.RunAndCollect);
    ModuleBuilder module = assembly.DefineDynamicModule(Emit.DynamicAssembly.AssemblyName);
    module.IgnoreAccessChecksTo("CodeArchitects.Platform.Actors.UnitTests");
    return module;
  }
}
