using CodeArchitects.Platform.Common;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Testing;

internal class FakeILGenerator : IILGenerator
{
  private readonly List<FakeInstruction> _instructions;
  private readonly List<FakeLocalBuilder> _locals;
  private readonly List<FakeLabel> _labels;
  private int _cursor;

  public FakeILGenerator()
  {
    _instructions = new();
    _locals = new();
    _labels = new();
  }

  public IReadOnlyList<FakeLabel> Labels => _labels;

  public void VerifyLocals(IEnumerable<FakeLocalBuilder> locals)
  {
    if (!_locals.SequenceEqual(locals))
      throw new Exception("Invalid locals.");
  }

  public void VerifyLocals(params FakeLocalBuilder[] locals)
  {
    VerifyLocals(locals as IEnumerable<FakeLocalBuilder>);
  }

  public void VerifyIL(Action<ILVerifier> verify)
  {
    ILVerifier verifier = new(_instructions);
    verify(verifier);
    verifier.VerifyComplete();
  }

  ILocalBuilder IILGenerator.DeclareLocal(Type localType)
  {
    return ((IILGenerator)this).DeclareLocal(localType, false);
  }

  ILocalBuilder IILGenerator.DeclareLocal(Type localType, bool pinned)
  {
    FakeLocalBuilder local = new(_locals.Count, localType, pinned);
    _locals.Add(local);
    return local;
  }

  ILabel IILGenerator.DefineLabel()
  {
    FakeLabel label = new();
    _labels.Add(label);
    return label;
  }

  void IILGenerator.Emit(OpCode opcode, Type cls)
  {
    AddInstruction(opcode, cls, 4);
  }

  void IILGenerator.Emit(OpCode opcode, string str)
  {
    AddInstruction(opcode, str, 4);
  }

  void IILGenerator.Emit(OpCode opcode, float arg)
  {
    AddInstruction(opcode, arg, 4);
  }

  void IILGenerator.Emit(OpCode opcode, sbyte arg)
  {
    AddInstruction(opcode, arg, 4);
  }

  void IILGenerator.Emit(OpCode opcode, MethodInfo meth)
  {
    AddInstruction(opcode, meth, 4);
  }

  void IILGenerator.Emit(OpCode opcode, FieldInfo field)
  {
    AddInstruction(opcode, field, 4);
  }

  void IILGenerator.Emit(OpCode opcode, ILocalBuilder local)
  {
    AddInstruction(opcode, (FakeLocalBuilder)local, 4);
  }

  void IILGenerator.Emit(OpCode opcode, ConstructorInfo con)
  {
    AddInstruction(opcode, con, 4);
  }

  void IILGenerator.Emit(OpCode opcode, long arg)
  {
    AddInstruction(opcode, arg, 4);
  }

  void IILGenerator.Emit(OpCode opcode, int arg)
  {
    AddInstruction(opcode, arg, 4);
  }

  void IILGenerator.Emit(OpCode opcode, short arg)
  {
    AddInstruction(opcode, arg, 4);
  }

  void IILGenerator.Emit(OpCode opcode, double arg)
  {
    AddInstruction(opcode, arg, 4);
  }

  void IILGenerator.Emit(OpCode opcode, byte arg)
  {
    AddInstruction(opcode, arg, 1);
  }

  void IILGenerator.Emit(OpCode opcode)
  {
    AddInstruction(opcode);
  }

  void IILGenerator.Emit(OpCode opcode, ILabel label)
  {
    AddInstruction(opcode, (FakeLabel)label, 1);
  }

  void IILGenerator.MarkLabel(ILabel loc)
  {
    FakeLabel label = (FakeLabel)loc;
    Debug.Assert(label.Position == -1, "Label was already marked.");
    label.Position = _cursor;
  }

  void IILGenerator.ThrowException(Type excType)
  {
    ((IILGenerator)this).Emit(OpCodes.Newobj, excType.GetRequiredConstructor());
    ((IILGenerator)this).Emit(OpCodes.Throw);
  }

  private void AddInstruction(OpCode opcode)
  {
    _instructions.Add(new(_cursor, opcode, default));
    _cursor += opcode.Size;
  }

  private void AddInstruction(OpCode opcode, Optional<object?> argument, int argSize)
  {
    _instructions.Add(new(_cursor, opcode, argument));
    _cursor += opcode.Size + argSize;
  }
}
