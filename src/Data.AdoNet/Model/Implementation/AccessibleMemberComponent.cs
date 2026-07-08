extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class AccessibleMemberComponent<T> : MemberComponent<T>
{
  private CollectionAccessor? _collectionAccessor;
  private string? _name;

  protected AccessibleMemberComponent(Getter<T> getValue, Setter<T> setValue)
  {
    GetValue = getValue;
    SetValue = setValue;
  }

  [AllowNull]
  public virtual string Name
  {
    get => _name ?? Member.Name;
    set => _name = value;
  }

  public new abstract MemberInfo Member { get; }

  public new Getter<T> GetValue { get; }

  public new Setter<T> SetValue { get; }

  protected override MemberInfo? MemberCore => Member;

  protected override Getter<T>? GetValueCore => GetValue;

  protected override Setter<T>? SetValueCore => SetValue;

  protected override bool HasMemberCore => true;

  public override ICollectionAccessor? CollectionAccessor => _collectionAccessor ??= Implementation.CollectionAccessor.Create(this);

  public static AccessibleMemberComponent<T> Create(MemberInfo member)
  {
    return member switch
    {
      PropertyInfo property => PropertyMemberComponent<T>.Create(property),
      FieldInfo field       => FieldMemberComponent<T>.Create(field),
      _                     => throw Errors.Unreachable
    };
  }
}
