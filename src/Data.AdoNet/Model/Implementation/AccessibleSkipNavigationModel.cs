using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class AccessibleSkipNavigationModel : SkipNavigationModel, IAccessibleSkipNavigationModel
{
  private readonly AccessibleMemberComponent<object?> _memberComponent;

  public AccessibleSkipNavigationModel(AccessibleMemberComponent<object?> memberComponent, int id, EntityModel from, EntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent, JoinEntityModel joinEntity)
    : base(id, from, to, associationKind, collectionKind, isOnDependent, joinEntity)
  {
    CollectionAccessor = memberComponent.CollectionAccessor ?? throw new ArgumentException("Expected a collection member component.", nameof(_memberComponent));
    _memberComponent = memberComponent;
  }

  protected override MemberComponent<object?> MemberComponent => _memberComponent;

  public new MemberInfo Member => _memberComponent.Member;

  public new Getter<object?> GetValue => _memberComponent.GetValue;

  public new Setter<object?> SetValue => _memberComponent.SetValue;

  public new ICollectionAccessor CollectionAccessor { get; }
}
