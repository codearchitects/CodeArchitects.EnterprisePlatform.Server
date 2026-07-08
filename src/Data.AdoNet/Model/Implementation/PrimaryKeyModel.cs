extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.Exceptions;
using CaPlatformCommon.System.Reflection;
using CaPlatformCommon.System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class PrimaryKeyModel : IPrimaryKeyModel
{
  private static readonly MethodInfo s_create1Method = typeof(PrimaryKeyModel).GetRequiredMethod(
    name: nameof(Create1),
    bindingAttr: BindingFlags.Static | BindingFlags.NonPublic);
  
  private static readonly MethodInfo s_create2Method = typeof(PrimaryKeyModel).GetRequiredMethod(
    name: nameof(Create2),
    bindingAttr: BindingFlags.Static | BindingFlags.NonPublic);
  
  private static readonly MethodInfo s_create3Method = typeof(PrimaryKeyModel).GetRequiredMethod(
    name: nameof(Create3),
    bindingAttr: BindingFlags.Static | BindingFlags.NonPublic);
  
  private static readonly MethodInfo s_create4Method = typeof(PrimaryKeyModel).GetRequiredMethod(
    name: nameof(Create4),
    bindingAttr: BindingFlags.Static | BindingFlags.NonPublic);

  private readonly List<IPrimaryKeyColumnModel> _columns;

  protected PrimaryKeyModel()
  {
    _columns = new();
  }

  protected abstract Getter<object?> GetValueCore { get; }

  protected abstract Setter<object?> SetValueCore { get; }

  public abstract bool IsComposite { get; }

  public abstract Type Type { get; }

  public IReadOnlyList<IPrimaryKeyColumnModel> Columns => _columns;

  public Getter<object?> GetValue => GetValueCore;

  public Setter<object?> SetValue => SetValueCore;

  public void AddColumn(IPrimaryKeyColumnModel column)
  {
    _columns.Add(column);
  }

  public bool TryGetColumn(ReadOnlySpan<char> name, [NotNullWhen(true)] out IPrimaryKeyColumnModel? column)
  {
    foreach (IPrimaryKeyColumnModel col in _columns)
    {
      if (name.SequenceEqual(col.Member.Name))
      {
        column = col;
        return true;
      }
    }

    column = null;
    return false;
  }

  public static PrimaryKeyModel Create(IReadOnlyList<(MemberInfo Member, Type Type)> membersAndTypes)
  {
    Debug.Assert(membersAndTypes.Count <= 4, "Expected at most 4 members.");

    object? result = membersAndTypes.Count switch
    {
      1 => s_create1Method.MakeGenericMethod(membersAndTypes.Map(item => item.Type)).Invoke(null, new object[] { membersAndTypes }),
      2 => s_create2Method.MakeGenericMethod(membersAndTypes.Map(item => item.Type)).Invoke(null, new object[] { membersAndTypes }),
      3 => s_create3Method.MakeGenericMethod(membersAndTypes.Map(item => item.Type)).Invoke(null, new object[] { membersAndTypes }),
      4 => s_create4Method.MakeGenericMethod(membersAndTypes.Map(item => item.Type)).Invoke(null, new object[] { membersAndTypes }),
      _ => Errors.Unreachable
    };

    return (PrimaryKeyModel)result!;
  }

  private static SimplePrimaryKeyModel<TKey> Create1<TKey>(IReadOnlyList<(MemberInfo Member, Type Type)> membersAndTypes)
    where TKey : IEquatable<TKey>
  {
    AccessibleMemberComponent<TKey> memberComponent = AccessibleMemberComponent<TKey>.Create(membersAndTypes[0].Member);

    return new(memberComponent.GetValue, memberComponent.SetValue);
  }

  private static CompositePrimaryKeyModel<TKey1, TKey2> Create2<TKey1, TKey2>(IReadOnlyList<(MemberInfo Member, Type Type)> membersAndTypes)
  {
    AccessibleMemberComponent<TKey1> memberComponent1 = AccessibleMemberComponent<TKey1>.Create(membersAndTypes[0].Member);
    AccessibleMemberComponent<TKey2> memberComponent2 = AccessibleMemberComponent<TKey2>.Create(membersAndTypes[1].Member);

    Getter<(TKey1, TKey2)> getValue = instance => (
      memberComponent1.GetValue(instance),
      memberComponent2.GetValue(instance));

    Setter<(TKey1, TKey2)> setValue = (instance, value) =>
    {
      memberComponent1.SetValue(instance, value.Item1);
      memberComponent2.SetValue(instance, value.Item2);
    };

    return new(getValue, setValue);
  }

  private static CompositePrimaryKeyModel<TKey1, TKey2, TKey3> Create3<TKey1, TKey2, TKey3>(IReadOnlyList<(MemberInfo Member, Type Type)> membersAndTypes)
  {
    AccessibleMemberComponent<TKey1> memberComponent1 = AccessibleMemberComponent<TKey1>.Create(membersAndTypes[0].Member);
    AccessibleMemberComponent<TKey2> memberComponent2 = AccessibleMemberComponent<TKey2>.Create(membersAndTypes[1].Member);
    AccessibleMemberComponent<TKey3> memberComponent3 = AccessibleMemberComponent<TKey3>.Create(membersAndTypes[2].Member);

    Getter<(TKey1, TKey2, TKey3)> getValue = instance => (
      memberComponent1.GetValue(instance),
      memberComponent2.GetValue(instance),
      memberComponent3.GetValue(instance));

    Setter<(TKey1, TKey2, TKey3)> setValue = (instance, value) =>
    {
      memberComponent1.SetValue(instance, value.Item1);
      memberComponent2.SetValue(instance, value.Item2);
      memberComponent3.SetValue(instance, value.Item3);
    };

    return new(getValue, setValue);
  }

  private static CompositePrimaryKeyModel<TKey1, TKey2, TKey3, TKey4> Create4<TKey1, TKey2, TKey3, TKey4>(IReadOnlyList<(MemberInfo Member, Type Type)> membersAndTypes)
  {
    AccessibleMemberComponent<TKey1> memberComponent1 = AccessibleMemberComponent<TKey1>.Create(membersAndTypes[0].Member);
    AccessibleMemberComponent<TKey2> memberComponent2 = AccessibleMemberComponent<TKey2>.Create(membersAndTypes[1].Member);
    AccessibleMemberComponent<TKey3> memberComponent3 = AccessibleMemberComponent<TKey3>.Create(membersAndTypes[2].Member);
    AccessibleMemberComponent<TKey4> memberComponent4 = AccessibleMemberComponent<TKey4>.Create(membersAndTypes[3].Member);

    Getter<(TKey1, TKey2, TKey3, TKey4)> getValue = instance => (
      memberComponent1.GetValue(instance),
      memberComponent2.GetValue(instance),
      memberComponent3.GetValue(instance),
      memberComponent4.GetValue(instance));

    Setter<(TKey1, TKey2, TKey3, TKey4)> setValue = (instance, value) =>
    {
      memberComponent1.SetValue(instance, value.Item1);
      memberComponent2.SetValue(instance, value.Item2);
      memberComponent3.SetValue(instance, value.Item3);
      memberComponent4.SetValue(instance, value.Item4);
    };

    return new(getValue, setValue);
  }
}
