using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Testing;

internal class FakeILGenerator : IILGenerator
{
  private readonly List<FakeInstruction> _instructions;
  private readonly List<FakeLocalBuilder> _locals;
  private readonly List<FakeLabel> _labels;

  public FakeILGenerator()
  {
    _instructions = new();
    _locals = new();
    _labels = new();
  }

  public void VerifyLocals(Action<LocalVerifier> verify)
  {
    LocalVerifier verifier = new(_locals);
    verify(verifier);
    verifier.VerifyComplete();
  }

  public void VerifyNoLocals()
  {
    LocalVerifier verifier = new(_locals);
    verifier.VerifyComplete();
  }

  public void VerifyIL(Action<ILVerifier> verify)
  {
    FakeLabelMarker marker = new(_labels);
    verify(marker);

    InstructionVerifier verifier = new(_instructions, marker.MarkedLabels);
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
    AddInstruction(opcode, cls);
  }

  void IILGenerator.Emit(OpCode opcode, string str)
  {
    AddInstruction(opcode, str);
  }

  void IILGenerator.Emit(OpCode opcode, float arg)
  {
    AddInstruction(opcode, arg);
  }

  void IILGenerator.Emit(OpCode opcode, sbyte arg)
  {
    AddInstruction(opcode, arg);
  }

  void IILGenerator.Emit(OpCode opcode, MethodInfo meth)
  {
    AddInstruction(opcode, meth);
  }

  void IILGenerator.Emit(OpCode opcode, FieldInfo field)
  {
    AddInstruction(opcode, field);
  }

  void IILGenerator.Emit(OpCode opcode, ILocalBuilder local)
  {
    AddInstruction(opcode, local);
  }

  void IILGenerator.Emit(OpCode opcode, ConstructorInfo con)
  {
    AddInstruction(opcode, con);
  }

  void IILGenerator.Emit(OpCode opcode, long arg)
  {
    AddInstruction(opcode, arg);
  }

  void IILGenerator.Emit(OpCode opcode, int arg)
  {
    AddInstruction(opcode, arg);
  }

  void IILGenerator.Emit(OpCode opcode, short arg)
  {
    AddInstruction(opcode, arg);
  }

  void IILGenerator.Emit(OpCode opcode, double arg)
  {
    AddInstruction(opcode, arg);
  }

  void IILGenerator.Emit(OpCode opcode, byte arg)
  {
    AddInstruction(opcode, arg);
  }

  void IILGenerator.Emit(OpCode opcode)
  {
    AddInstruction(opcode);
  }

  void IILGenerator.Emit(OpCode opcode, ILabel label)
  {
    AddInstruction(opcode, label);
  }

  public void Emit(OpCode opcode, params ILabel[] labels)
  {
    AddInstruction(opcode, labels);
  }

  void IILGenerator.MarkLabel(ILabel loc)
  {
    FakeLabel label = (FakeLabel)loc;
    Debug.Assert(label.Position == -1, "Label was already marked.");
    label.Position = _instructions.Count;
  }

  void IILGenerator.ThrowException(Type excType)
  {
    ((IILGenerator)this).Emit(OpCodes.Newobj, excType.GetRequiredConstructor());
    ((IILGenerator)this).Emit(OpCodes.Throw);
  }

  private void AddInstruction(OpCode opcode)
  {
    _instructions.Add(new(opcode, default));
  }

  private void AddInstruction(OpCode opcode, object? argument)
  {
    _instructions.Add(new(opcode, argument));
  }
}
