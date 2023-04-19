using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Testing;

internal class FakeILGeneratorProvider : IILGeneratorProvider
{
  private readonly List<FakeILGenerator> _generators;
  private int _index;

  public FakeILGeneratorProvider()
  {
    _generators = new();
  }

  public FakeILGenerator AddGenerator()
  {
    FakeILGenerator generator = new();
    _generators.Add(generator);
    return generator;
  }

  public IILGenerator GetILGenerator(MethodBuilder methodBuilder)
  {
    methodBuilder.GetILGenerator().Emit(OpCodes.Ret);
    FakeILGenerator result = _generators[_index++];
    result.MethodName = methodBuilder.Name;
    return result;
  }

  public IILGenerator GetILGenerator(ConstructorBuilder constructorBuilder)
  {
    constructorBuilder.GetILGenerator().Emit(OpCodes.Ret);
    FakeILGenerator result = _generators[_index++];
    result.MethodName = constructorBuilder.Name;
    return result;
  }
}
