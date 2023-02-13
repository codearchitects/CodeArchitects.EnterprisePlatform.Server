using System.Reflection;

namespace CodeArchitects.Platform.Emit.Testing;

internal abstract class ILVerifier
{
  private readonly string _methodName;

  protected ILVerifier(string methodName)
  {
    _methodName = methodName;
  }

  protected ILVerifier MoveNext()
  {
    Index++;
    return this;
  }

  protected int Index { get; private set; }

  protected Exception Error(string message)
  {
    return new Exception($"Error at instruction #{Index + 1} of method {_methodName}: {message}");
  }

  public abstract ILVerifier Br(string label);

  public abstract ILVerifier Brtrue_S(string label);

  public abstract ILVerifier Br_S(string label);

  public abstract ILVerifier Call(Predicate<MethodBase> predicate);

  public abstract ILVerifier Call(MethodBase methodBase);

  public abstract ILVerifier Call(Type declaringType, string methodName, Type[] parameterTypes);

  public abstract ILVerifier Call(Type declaringType, string methodName, Type[] typeArguments, Type[] parameterTypes);

  public abstract ILVerifier Callvirt(Predicate<MethodBase> predicate);

  public abstract ILVerifier Callvirt(MethodBase methodBase);

  public abstract ILVerifier Callvirt(Type declaringType, string methodName, Type[] parameterTypes);

  public abstract ILVerifier Callvirt(Type declaringType, string methodName, Type[] typeArguments, Type[] parameterTypes);

  public abstract ILVerifier CastClass(Type type);

  public abstract ILVerifier Dup();

  public abstract ILVerifier Ldarg_0();

  public abstract ILVerifier Ldarg_1();

  public abstract ILVerifier Ldarg_2();

  public abstract ILVerifier Ldarg_3();

  public abstract ILVerifier Ldarg_S(int value);

  public abstract ILVerifier Ldc_I4_1();

  public abstract ILVerifier Ldc_I4_2();

  public abstract ILVerifier Ldc_I4_3();

  public abstract ILVerifier Ldc_I4_4();

  public abstract ILVerifier Ldc_I4_5();

  public abstract ILVerifier Ldfld(Predicate<FieldInfo> predicate);

  public abstract ILVerifier Ldfld(FieldInfo field);

  public abstract ILVerifier Ldfld(string fieldName);

  public abstract ILVerifier Ldsfld(Predicate<FieldInfo> predicate);

  public abstract ILVerifier Ldsfld(FieldInfo field);

  public abstract ILVerifier Ldsfld(string fieldName);

  public abstract ILVerifier Ldloc_0();

  public abstract ILVerifier Ldloca_S(int index);

  public abstract ILVerifier Ldstr();

  public abstract ILVerifier Ldstr(string str);

  public abstract ILVerifier MarkLabel(string name);

  public abstract ILVerifier Newobj(Predicate<ConstructorInfo> predicate);

  public abstract ILVerifier Newobj(Type declaringType, Type[] parameterTypes);

  public abstract ILVerifier Ret();

  public abstract ILVerifier Stfld(Predicate<FieldInfo> predicate);

  public abstract ILVerifier Stfld(FieldInfo field);

  public abstract ILVerifier Stfld(string fieldName);

  public abstract ILVerifier Stsfld(Predicate<FieldInfo> predicate);

  public abstract ILVerifier Stsfld(FieldInfo field);

  public abstract ILVerifier Stsfld(string fieldName);

  public abstract ILVerifier Stloc_0();

  public abstract ILVerifier Switch(params string[] labels);

  public abstract ILVerifier Throw();
}
