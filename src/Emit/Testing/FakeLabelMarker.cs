using System.Reflection;

namespace CodeArchitects.Platform.Emit.Testing;

internal class FakeLabelMarker : ILVerifier
{
  private readonly List<FakeLabel> _labels;
  private readonly Dictionary<int, string> _markedLabels;

  public FakeLabelMarker(string methodName, List<FakeLabel> labels)
    : base(methodName)
  {
    _labels = labels;
    _markedLabels = new();
  }

  public IReadOnlyDictionary<int, string> MarkedLabels => _markedLabels;

  public override ILVerifier MarkLabel(string name)
  {
    if (_labels.Find(label => label.Position == Index) is not { } label)
      throw Error($"No label was marked before instruction #{Index}.");

    _markedLabels.Add(Index, name);

    return this;
  }

  #region No op

  public override ILVerifier Br(string label) => MoveNext();

  public override ILVerifier Brtrue_S(string label) => MoveNext();

  public override ILVerifier Br_S(string label) => MoveNext();

  public override ILVerifier Call(Predicate<MethodBase> predicate) => MoveNext();

  public override ILVerifier Call(MethodBase methodBase) => MoveNext();

  public override ILVerifier Call(Type declaringType, string methodName, Type[] parameterTypes) => MoveNext();

  public override ILVerifier Call(Type declaringType, string methodName, Type[] typeArguments, Type[] parameterTypes) => MoveNext();

  public override ILVerifier Callvirt(Predicate<MethodBase> predicate) => MoveNext();

  public override ILVerifier Callvirt(MethodBase methodBase) => MoveNext();

  public override ILVerifier Callvirt(Type declaringType, string methodName, Type[] parameterTypes) => MoveNext();

  public override ILVerifier Callvirt(Type declaringType, string methodName, Type[] typeArguments, Type[] parameterTypes) => MoveNext();

  public override ILVerifier CastClass(Type type) => MoveNext();

  public override ILVerifier Dup() => MoveNext();

  public override ILVerifier Ldarg_0() => MoveNext();

  public override ILVerifier Ldarg_1() => MoveNext();

  public override ILVerifier Ldarg_2() => MoveNext();

  public override ILVerifier Ldarg_3() => MoveNext();

  public override ILVerifier Ldarg_S(int value) => MoveNext();

  public override ILVerifier Ldc_I4_1() => MoveNext();

  public override ILVerifier Ldc_I4_2() => MoveNext();

  public override ILVerifier Ldc_I4_3() => MoveNext();

  public override ILVerifier Ldc_I4_4() => MoveNext();

  public override ILVerifier Ldc_I4_5() => MoveNext();

  public override ILVerifier Ldfld(Predicate<FieldInfo> predicate) => MoveNext();

  public override ILVerifier Ldfld(string fieldName) => MoveNext();

  public override ILVerifier Ldloc_0() => MoveNext();

  public override ILVerifier Ldloca_S(int index) => MoveNext();

  public override ILVerifier Ldsfld(Predicate<FieldInfo> predicate) => MoveNext();

  public override ILVerifier Ldsfld(string fieldName) => MoveNext();

  public override ILVerifier Ldstr() => MoveNext();

  public override ILVerifier Ldstr(string str) => MoveNext();

  public override ILVerifier Newobj(Predicate<ConstructorInfo> predicate) => MoveNext();

  public override ILVerifier Newobj(Type declaringType, Type[] parameterTypes) => MoveNext();

  public override ILVerifier Ret() => MoveNext();

  public override ILVerifier Stfld(Predicate<FieldInfo> predicate) => MoveNext();

  public override ILVerifier Stfld(string fieldName) => MoveNext();

  public override ILVerifier Stloc_0() => MoveNext();

  public override ILVerifier Stsfld(Predicate<FieldInfo> predicate) => MoveNext();

  public override ILVerifier Stsfld(string fieldName) => MoveNext();

  public override ILVerifier Switch(params string[] labels) => MoveNext();

  public override ILVerifier Throw() => MoveNext();

  #endregion
}
