using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit;

internal interface IILGenerator
{
  ILabel DefineLabel();
  ILocalBuilder DeclareLocal(Type localType);
  ILocalBuilder DeclareLocal(Type localType, bool pinned);
  void Emit(OpCode opcode, Type cls);
  void Emit(OpCode opcode, string str);
  void Emit(OpCode opcode, float arg);
  void Emit(OpCode opcode, sbyte arg);
  void Emit(OpCode opcode, MethodInfo meth);
  void Emit(OpCode opcode, FieldInfo field);
  void Emit(OpCode opcode, ILocalBuilder local);
  void Emit(OpCode opcode, ConstructorInfo con);
  void Emit(OpCode opcode, long arg);
  void Emit(OpCode opcode, int arg);
  void Emit(OpCode opcode, short arg);
  void Emit(OpCode opcode, double arg);
  void Emit(OpCode opcode, byte arg);
  void Emit(OpCode opcode);
  void Emit(OpCode opcode, ILabel label);
  void Emit(OpCode opcode, params ILabel[] labels);
  void MarkLabel(ILabel loc);
  void ThrowException(Type excType);
}
