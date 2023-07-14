using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Reflection;

internal class ReflectionILGeneratorProvider : IILGeneratorProvider
{
  public IILGenerator GetILGenerator(MethodBuilder methodBuilder)
  {
    return new ReflectionILGenerator(methodBuilder.GetILGenerator());
  }

  public IILGenerator GetILGenerator(ConstructorBuilder constructorBuilder)
  {
    return new ReflectionILGenerator(constructorBuilder.GetILGenerator());
  }

  public IILGenerator GetILGenerator(DynamicMethod dynamicMethod)
  {
    return new ReflectionILGenerator(dynamicMethod.GetILGenerator());
  }
}
