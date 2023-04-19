using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit;

internal interface IILGeneratorProvider
{
  IILGenerator GetILGenerator(MethodBuilder methodBuilder);
  IILGenerator GetILGenerator(ConstructorBuilder constructorBuilder);
}
