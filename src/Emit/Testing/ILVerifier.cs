using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Testing;

internal class ILVerifier
{
  private readonly IReadOnlyList<FakeInstruction> _instructions;
  private int _index;

  public ILVerifier(IReadOnlyList<FakeInstruction> instructions)
  {
    _instructions = instructions;
  }

  public ILVerifier Call(Predicate<MethodBase>? predicate = null)
  {
    return predicate is null
      ? Verify(OpCodes.Call)
      : Verify(OpCodes.Call, predicate);
  }

  public ILVerifier Call(Type declaringType, string methodName, Type[] parameterTypes)
  {
    return Verify(OpCodes.Call, (MethodBase method) =>
    {
      if (method.DeclaringType != declaringType)
        return false;
      if (method.Name != methodName)
        return false;

      ParameterInfo[] parameters = method.GetParameters();
      if (parameters.Length != parameterTypes.Length)
        return false;

      for (int i = 0; i < parameters.Length; i++)
      {
        if (parameters[i].ParameterType != parameterTypes[i])
          return false;
      }

      return true;
    });
  }

  public ILVerifier Callvirt(Predicate<MethodBase>? predicate = null)
  {
    return predicate is null
      ? Verify(OpCodes.Callvirt)
      : Verify(OpCodes.Callvirt, predicate);
  }

  public ILVerifier Callvirt(Type declaringType, string methodName, Type[] parameterTypes)
  {
    return Verify(OpCodes.Callvirt, (MethodBase method) =>
    {
      if (method.DeclaringType != declaringType)
        return false;
      if (method.Name != methodName)
        return false;

      ParameterInfo[] parameters = method.GetParameters();
      if (parameters.Length != parameterTypes.Length)
        return false;

      for (int i = 0; i < parameters.Length; i++)
      {
        if (parameters[i].ParameterType != parameterTypes[i])
          return false;
      }

      return true;
    });
  }

  public ILVerifier Dup()
  {
    return Verify(OpCodes.Dup);
  }

  public ILVerifier Ldarg_0()
  {
    return Verify(OpCodes.Ldarg_0);
  }

  public ILVerifier Ldarg_1()
  {
    return Verify(OpCodes.Ldarg_1);
  }

  public ILVerifier Ldarg_2()
  {
    return Verify(OpCodes.Ldarg_2);
  }

  public ILVerifier Ldarg_3()
  {
    return Verify(OpCodes.Ldarg_3);
  }

  public ILVerifier Ldarg_S(int value)
  {
    return Verify(OpCodes.Ldarg_S, value);
  }

  public ILVerifier Ldfld(Predicate<FieldInfo>? predicate = null)
  {
    return predicate is null
      ? Verify(OpCodes.Ldfld)
      : Verify(OpCodes.Ldfld, predicate);
  }

  public ILVerifier Ldfld(Type fieldType, string fieldName)
  {
    return Verify(OpCodes.Ldfld, field => field.FieldType == fieldType && field.Name == fieldName);
  }

  public ILVerifier Ldsfld(Predicate<FieldInfo>? predicate = null)
  {
    return predicate is null
      ? Verify(OpCodes.Ldsfld)
      : Verify(OpCodes.Ldsfld, predicate);
  }

  public ILVerifier Ldsfld(Type fieldType, string fieldName)
  {
    return Verify(OpCodes.Ldsfld, field => field.FieldType == fieldType && field.Name == fieldName);
  }

  public ILVerifier Newobj(Predicate<ConstructorInfo>? predicate = null)
  {
    return predicate is null
      ? Verify(OpCodes.Newobj)
      : Verify(OpCodes.Newobj, predicate);
  }

  public ILVerifier Newobj(Type declaringType, Type[] parameterTypes)
  {
    return Verify(OpCodes.Newobj, (ConstructorInfo ctor) =>
    {
      if (ctor.DeclaringType != declaringType)
        return false;

      ParameterInfo[] parameters = ctor.GetParameters();
      if (parameters.Length != parameterTypes.Length)
        return false;

      for (int i = 0; i < parameters.Length; i++)
      {
        if (parameters[i].ParameterType != parameterTypes[i])
          return false;
      }

      return true;
    });
  }

  public ILVerifier Ret()
  {
    return Verify(OpCodes.Ret);
  }

  public ILVerifier Stfld(Predicate<FieldInfo>? predicate = null)
  {
    return predicate is null
      ? Verify(OpCodes.Stfld)
      : Verify(OpCodes.Stfld, predicate);
  }

  public ILVerifier Stfld(Type fieldType, string fieldName)
  {
    return Verify(OpCodes.Stfld, field => field.FieldType == fieldType && field.Name == fieldName);
  }

  public ILVerifier Stsfld(Predicate<FieldInfo>? predicate = null)
  {
    return predicate is null
      ? Verify(OpCodes.Stsfld)
      : Verify(OpCodes.Stsfld, predicate);
  }

  public ILVerifier Stsfld(Type fieldType, string fieldName)
  {
    return Verify(OpCodes.Stsfld, field => field.FieldType == fieldType && field.Name == fieldName);
  }

  private ILVerifier Verify(OpCode opcode)
  {
    FakeInstruction instruction = _instructions[_index++];

    VerifyOpCode(instruction, opcode);

    if (instruction.Argument.HasValue)
      throw Error(instruction.Position, $"Expected the instruction to not have an argument but found '{instruction.Argument.Value}'.");

    return this;
  }

  private ILVerifier Verify(OpCode opcode, Predicate<FieldInfo> predicate)
  {
    FakeInstruction instruction = _instructions[_index++];

    VerifyOpCode(instruction, opcode);

    VerifyHasArgument(instruction);

    object? value = instruction.Argument.Value;
    if (value is not FieldInfo field)
      throw Error(instruction.Position, $"Expected the instruction to have a FieldInfo argument but found '{value ?? "<null>"}'.");

    if (!predicate(field))
      throw Error(instruction.Position, "The FieldInfo argument does not match the provided predicate.");

    return this;
  }

  private ILVerifier Verify(OpCode opcode, Predicate<MethodBase> predicate)
  {
    FakeInstruction instruction = _instructions[_index++];

    VerifyOpCode(instruction, opcode);

    VerifyHasArgument(instruction);

    object? value = instruction.Argument.Value;
    if (value is not MethodBase method)
      throw Error(instruction.Position, $"Expected the instruction to have a MethodInfo argument but found '{value ?? "<null>"}'.");

    if (!predicate(method))
      throw Error(instruction.Position, "The MethodInfo argument does not match the provided predicate.");

    return this;
  }

  private ILVerifier Verify(OpCode opcode, Predicate<ConstructorInfo> predicate)
  {
    FakeInstruction instruction = _instructions[_index++];

    VerifyOpCode(instruction, opcode);

    VerifyHasArgument(instruction);

    object? value = instruction.Argument.Value;
    if (value is not ConstructorInfo method)
      throw Error(instruction.Position, $"Expected the instruction to have a ConstructorInfo argument but found '{value ?? "<null>"}'.");

    if (!predicate(method))
      throw Error(instruction.Position, "The ConstructorInfo argument does not match the provided predicate.");

    return this;
  }

  private ILVerifier Verify(OpCode opcode, Predicate<Type> predicate)
  {
    FakeInstruction instruction = _instructions[_index++];

    VerifyOpCode(instruction, opcode);

    VerifyHasArgument(instruction);

    object? value = instruction.Argument.Value;
    if (value is not Type type)
      throw Error(instruction.Position, $"Expected the instruction to have a Type argument but found '{value ?? "<null>"}'.");

    if (!predicate(type))
      throw Error(instruction.Position, "The Type argument does not match the provided predicate.");

    return this;
  }

  private ILVerifier Verify(OpCode opcode, Predicate<FakeLabel> predicate)
  {
    FakeInstruction instruction = _instructions[_index++];

    VerifyOpCode(instruction, opcode);

    VerifyHasArgument(instruction);

    object? value = instruction.Argument.Value;
    if (value is not FakeLabel label)
      throw Error(instruction.Position, $"Expected the instruction to have a FakeLabel argument but found '{value ?? "<null>"}'.");

    if (!predicate(label))
      throw Error(instruction.Position, "The FakeLabel argument does not match the provided predicate.");

    return this;
  }

  private ILVerifier Verify(OpCode opcode, Predicate<FakeLocalBuilder> predicate)
  {
    FakeInstruction instruction = _instructions[_index++];

    VerifyOpCode(instruction, opcode);

    VerifyHasArgument(instruction);

    object? value = instruction.Argument.Value;
    if (value is not FakeLocalBuilder localBuilder)
      throw Error(instruction.Position, $"Expected the instruction to have a FakeLocalBuilder argument but found '{value ?? "<null>"}'.");

    if (!predicate(localBuilder))
      throw Error(instruction.Position, "The FakeLocalBuilder argument does not match the provided predicate.");

    return this;
  }

  private ILVerifier Verify(OpCode opcode, int expected)
  {
    return VerifyValue(opcode, expected);
  }

  private ILVerifier Verify(OpCode opcode, short expected)
  {
    return VerifyValue(opcode, expected);
  }

  private ILVerifier Verify(OpCode opcode, long expected)
  {
    return VerifyValue(opcode, expected);
  }

  private ILVerifier Verify(OpCode opcode, float expected)
  {
    return VerifyValue(opcode, expected);
  }

  private ILVerifier Verify(OpCode opcode, double expected)
  {
    return VerifyValue(opcode, expected);
  }

  private ILVerifier Verify(OpCode opcode, byte expected)
  {
    return VerifyValue(opcode, expected);
  }

  private ILVerifier Verify(OpCode opcode, sbyte expected)
  {
    return VerifyValue(opcode, expected);
  }

  private ILVerifier Verify(OpCode opcode, string expected)
  {
    return VerifyValue(opcode, expected);
  }

  private ILVerifier VerifyValue<T>(OpCode opcode, T expected)
    where T : IEquatable<T>
  {
    FakeInstruction instruction = _instructions[_index++];

    VerifyOpCode(instruction, opcode);

    VerifyHasArgument(instruction);

    object? value = instruction.Argument.Value;
    if (value is not T actual || !expected.Equals(actual))
      throw Error(instruction.Position, $"Expected '{expected}' as argument, but got '{value ?? "<null>"}'.");

    return this;
  }

  private void VerifyOpCode(FakeInstruction instruction, OpCode opcode)
  {
    if (instruction.OpCode != opcode)
      throw Error(instruction.Position, $"Expected opcode '{opcode}' but found '{instruction.OpCode}'.");
  }

  private void VerifyHasArgument(FakeInstruction instruction)
  {
    if (!instruction.Argument.HasValue)
      throw Error(instruction.Position, "Expected the instruction to have an argument but found none.");
  }

  private Exception Error(int position, string message) => new Exception($"Error at IL_{position:x4}: {message}");
}
