using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Reflection;

internal class ReflectionILGenerator : IILGenerator
{
  private readonly ILGenerator _il;

  public ReflectionILGenerator(ILGenerator il)
  {
    _il = il;
  }

  public ILocalBuilder DeclareLocal(Type localType)
  {
    return new ReflectionLocalBuilder(_il.DeclareLocal(localType));
  }

  public ILocalBuilder DeclareLocal(Type localType, bool pinned)
  {
    return new ReflectionLocalBuilder(_il.DeclareLocal(localType, pinned));
  }

  public ILabel DefineLabel()
  {
    return new ReflectionLabel(_il.DefineLabel());
  }

  public void Emit(OpCode opcode, Type cls)
  {
    _il.Emit(opcode, cls);
  }

  public void Emit(OpCode opcode, string str)
  {
    _il.Emit(opcode, str);
  }

  public void Emit(OpCode opcode, float arg)
  {
    _il.Emit(opcode, arg);
  }

  public void Emit(OpCode opcode, sbyte arg)
  {
    _il.Emit(opcode, arg);
  }

  public void Emit(OpCode opcode, MethodInfo meth)
  {
    _il.Emit(opcode, meth);
  }

  public void Emit(OpCode opcode, FieldInfo field)
  {
    _il.Emit(opcode, field);
  }

  public void Emit(OpCode opcode, ILocalBuilder local)
  {
    _il.Emit(opcode, ((ReflectionLocalBuilder)local).Builder);
  }

  public void Emit(OpCode opcode, ConstructorInfo con)
  {
    _il.Emit(opcode, con);
  }

  public void Emit(OpCode opcode, long arg)
  {
    _il.Emit(opcode, arg);
  }

  public void Emit(OpCode opcode, int arg)
  {
    _il.Emit(opcode, arg);
  }

  public void Emit(OpCode opcode, short arg)
  {
    _il.Emit(opcode, arg);
  }

  public void Emit(OpCode opcode, double arg)
  {
    _il.Emit(opcode, arg);
  }

  public void Emit(OpCode opcode, byte arg)
  {
    _il.Emit(opcode, arg);
  }

  public void Emit(OpCode opcode)
  {
    _il.Emit(opcode);
  }

  public void Emit(OpCode opcode, ILabel label)
  {
    _il.Emit(opcode, ((ReflectionLabel)label).Label);
  }

  public void Emit(OpCode opcode, params ILabel[] labels)
  {
    _il.Emit(opcode, labels.Map(label => ((ReflectionLabel)label).Label));
  }

  public void MarkLabel(ILabel loc)
  {
    _il.MarkLabel(((ReflectionLabel)loc).Label);
  }

  public void ThrowException(Type excType)
  {
    _il.ThrowException(excType);
  }
}
