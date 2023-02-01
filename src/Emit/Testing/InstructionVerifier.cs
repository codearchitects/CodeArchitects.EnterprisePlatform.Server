using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Testing;

internal class InstructionVerifier : ILVerifier
{
  private readonly IReadOnlyList<FakeInstruction> _instructions;
  private readonly IReadOnlyDictionary<int, string> _markedLabels;

  public InstructionVerifier(IReadOnlyList<FakeInstruction> instructions, IReadOnlyDictionary<int, string> markedLabels)
  {
    _instructions = instructions;
    _markedLabels = markedLabels;
  }

  public void VerifyComplete()
  {
    if (Index != _instructions.Count)
      throw new Exception($"Remaining {_instructions.Count - Index} instructions were not verified.");
  }

  public override ILVerifier Br(string label)
  {
    VerifyOpCode(OpCodes.Br, out ILabel fakeLabel);

    VerifyLabel(fakeLabel, label);

    return MoveNext();
  }

  public override ILVerifier Brtrue_S(string label)
  {
    VerifyOpCode(OpCodes.Brtrue_S, out ILabel fakeLabel);

    VerifyLabel(fakeLabel, label);

    return MoveNext();
  }

  public override ILVerifier Br_S(string label)
  {
    VerifyOpCode(OpCodes.Br_S, out ILabel fakeLabel);

    VerifyLabel(fakeLabel, label);

    return MoveNext();
  }

  public override ILVerifier Call(Predicate<MethodBase> predicate)
  {
    return Verify(OpCodes.Call, predicate);
  }

  public override ILVerifier Call(MethodBase methodBase)
  {
    return Verify(OpCodes.Call, (MethodBase method) => method.Equals(methodBase));
  }

  public override ILVerifier Call(Type declaringType, string methodName, Type[] parameterTypes)
  {
    return VerifyInvocation(OpCodes.Call, declaringType, methodName, Type.EmptyTypes, parameterTypes);
  }

  public override ILVerifier Call(Type declaringType, string methodName, Type[] typeArguments, Type[] parameterTypes)
  {
    return VerifyInvocation(OpCodes.Call, declaringType, methodName, typeArguments, parameterTypes);
  }

  public override ILVerifier Callvirt(Predicate<MethodBase> predicate)
  {
    return Verify(OpCodes.Callvirt, predicate);
  }

  public override ILVerifier Callvirt(Type declaringType, string methodName, Type[] parameterTypes)
  {
    return VerifyInvocation(OpCodes.Callvirt, declaringType, methodName, Type.EmptyTypes, parameterTypes);
  }

  public override ILVerifier Callvirt(Type declaringType, string methodName, Type[] typeArguments, Type[] parameterTypes)
  {
    return VerifyInvocation(OpCodes.Callvirt, declaringType, methodName, typeArguments, parameterTypes);
  }

  public override ILVerifier Dup()
  {
    VerifyOpCode(OpCodes.Dup);

    return MoveNext();
  }

  public override ILVerifier Ldarg_0()
  {
    VerifyOpCode(OpCodes.Ldarg_0);

    return MoveNext();
  }

  public override ILVerifier Ldarg_1()
  {
    VerifyOpCode(OpCodes.Ldarg_1);

    return MoveNext();
  }

  public override ILVerifier Ldarg_2()
  {
    VerifyOpCode(OpCodes.Ldarg_2);

    return MoveNext();
  }

  public override ILVerifier Ldarg_3()
  {
    VerifyOpCode(OpCodes.Ldarg_3);

    return MoveNext();
  }

  public override ILVerifier Ldarg_S(int value)
  {
    return Verify(OpCodes.Ldarg_S, value);
  }

  public override ILVerifier Ldfld(Predicate<FieldInfo> predicate)
  {
    return Verify(OpCodes.Ldfld, predicate);
  }

  public override ILVerifier Ldfld(string fieldName)
  {
    return Verify(OpCodes.Ldfld, (FieldInfo field) => field.Name == fieldName);
  }

  public override ILVerifier Ldsfld(Predicate<FieldInfo> predicate)
  {
    return Verify(OpCodes.Ldsfld, predicate);
  }

  public override ILVerifier Ldsfld(string fieldName)
  {
    return Verify(OpCodes.Ldsfld, (FieldInfo field) => field.Name == fieldName);
  }

  public override ILVerifier Ldloc_0()
  {
    VerifyOpCode(OpCodes.Ldloc_0);

    return MoveNext();
  }

  public override ILVerifier Ldstr()
  {
    VerifyOpCode(OpCodes.Ldstr);

    return MoveNext();
  }

  public override ILVerifier Ldstr(string str)
  {
    return Verify(OpCodes.Ldstr, str);
  }

  public override ILVerifier MarkLabel(string name)
  {
    return this;
  }

  public override ILVerifier Newobj(Predicate<ConstructorInfo> predicate)
  {
    return Verify(OpCodes.Newobj, predicate);
  }

  public override ILVerifier Newobj(Type declaringType, Type[] parameterTypes)
  {
    return VerifyInvocation(OpCodes.Newobj, declaringType, ConstructorInfo.ConstructorName, Type.EmptyTypes, parameterTypes);
  }

  public override ILVerifier Ret()
  {
    VerifyOpCode(OpCodes.Ret);

    return MoveNext();
  }

  public override ILVerifier Stfld(Predicate<FieldInfo> predicate)
  {
    return Verify(OpCodes.Stfld, predicate);
  }

  public override ILVerifier Stfld(string fieldName)
  {
    return Verify(OpCodes.Stfld, (FieldInfo field) => field.Name == fieldName);
  }

  public override ILVerifier Stsfld(Predicate<FieldInfo> predicate)
  {
    return Verify(OpCodes.Stsfld, predicate);
  }

  public override ILVerifier Stsfld(string fieldName)
  {
    return Verify(OpCodes.Stsfld, (FieldInfo field) => field.Name == fieldName);
  }

  public override ILVerifier Stloc_0()
  {
    VerifyOpCode(OpCodes.Stloc_0);

    return MoveNext();
  }

  public override ILVerifier Switch(params string[] labels)
  {
    VerifyOpCode(OpCodes.Switch, out ILabel[] fakeLabels);

    if (labels.Length != fakeLabels.Length)
      throw Error($"Expected {labels.Length} labels, but found {fakeLabels.Length} instead.");

    for (int i = 0; i < labels.Length; i++)
    {
      VerifyLabel(fakeLabels[i], labels[i]);
    }

    return MoveNext();
  }

  public override ILVerifier Throw()
  {
    VerifyOpCode(OpCodes.Throw);

    return MoveNext();
  }



  private ILVerifier Verify(OpCode opcode, Predicate<FieldInfo> predicate)
  {
    VerifyOpCode(opcode, out FieldInfo field);

    if (!predicate(field))
      throw Error("The FieldInfo argument does not match the provided predicate.");

    return MoveNext();
  }

  private ILVerifier Verify(OpCode opcode, Predicate<MethodBase> predicate)
  {
    VerifyOpCode(opcode, out MethodBase method);

    if (!predicate(method))
      throw Error("The MethodInfo argument does not match the provided predicate.");

    return MoveNext();
  }

  private ILVerifier Verify(OpCode opcode, Predicate<ConstructorInfo> predicate)
  {
    VerifyOpCode(opcode, out ConstructorInfo constructor);

    if (!predicate(constructor))
      throw Error("The ConstructorInfo argument does not match the provided predicate.");

    return MoveNext();
  }

  private ILVerifier Verify(OpCode opcode, Predicate<Type> predicate)
  {
    VerifyOpCode(opcode, out Type type);

    if (!predicate(type))
      throw Error("The Type argument does not match the provided predicate.");

    return MoveNext();
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
    VerifyOpCode(opcode, out T value);

    if (!expected.Equals(value))
      throw Error($"Expected '{expected}' as argument, but got '{value}'.");

    return MoveNext();
  }

  private ILVerifier VerifyInvocation(OpCode opcode, Type declaringType, string methodName, Type[] typeArguments, Type[] parameterTypes)
  {
    return Verify(opcode, (MethodBase method) =>
    {
      if (method.DeclaringType != declaringType)
        return false;
      if (method.Name != methodName)
        return false;

      if (method.IsGenericMethod)
      {
        if (!method.GetGenericArguments().SequenceEqual(typeArguments))
          return false;
      }
      else
      {
        if (typeArguments.Length != 0)
          return false;
      }

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

  private void VerifyOpCode(OpCode opcode)
  {
    FakeInstruction instruction = _instructions[Index];

    if (instruction.OpCode != opcode)
      throw Error($"Expected opcode '{opcode}' but found '{instruction.OpCode}'.");

    if (instruction.Argument.HasValue)
      throw Error($"Expected the instruction to not have an argument but found '{instruction.Argument.Value}'.");
  }

  private void VerifyOpCode<T>(OpCode opcode, out T argument)
  {
    FakeInstruction instruction = _instructions[Index];

    if (instruction.OpCode != opcode)
      throw Error($"Expected opcode '{opcode}' but found '{instruction.OpCode}'.");

    if (!instruction.Argument.HasValue)
      throw Error("Expected the instruction to have an argument but found none.");

    object? argumentValue = instruction.Argument.Value;

    if (argumentValue is null)
      throw Error($"Expected the instruction to have an argument of type '{typeof(T).Name}', but found <null>.");

    if (argumentValue is not T value)
      throw Error($"Expected the instruction to have an argument of type '{typeof(T).Name}', but found '{argumentValue.GetType().Name}'.");

    argument = value;
  }

  private void VerifyLabel(ILabel label, string name)
  {
    int position = ((FakeLabel)label).Position;

    if (!_markedLabels.TryGetValue(position, out string labelName))
      throw Error($"Label at position {position} was not marked.");

    if (name != labelName)
      throw Error($"Expected label '{name}' but found '{labelName}'.");
  }
}
