using CodeArchitects.Platform.Emit;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Data.AdoNet.Fixtures;

internal static class DynamicAssembly
{
  public static ModuleBuilder CreateModule()
  {
    string name = "CodeArchitects.Platform.Data.AdoNet.UnitTests.Dynamic";

    AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndCollect);
    ModuleBuilder module = assembly.DefineDynamicModule(name);
    module.IgnoreAccessChecksTo("CodeArchitects.Platform.Data.AdoNet");
    module.IgnoreAccessChecksTo("CodeArchitects.Platform.Data.AdoNet.UnitTests");

    return module;
  }
}
