using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class OrdinaryColumnModel : ColumnModel, IOrdinaryColumnModel
{
  private readonly AccessibleMemberComponent _memberComponent;

  public OrdinaryColumnModel(AccessibleMemberComponent memberComponent, short index)
    : base(index)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent MemberComponent => _memberComponent;

  public override bool IsPrimaryKey => false;

  public override bool IsForeignKey => false;

  [AllowNull]
  public override string Name
  {
    get => _memberComponent.Name;
    set => _memberComponent.Name = value;
  }

  public new MemberInfo Member => _memberComponent.Member;

  public new Getter<object?> GetValue => _memberComponent.GetValue;

  public new Setter<object?> SetValue => _memberComponent.SetValue;

  public override TResult Accept<TVisitor, TResult>(in TVisitor visitor)
  {
    return visitor.VisitOrdinary(this);
  }

  public override TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
  {
    return visitor.VisitOrdinary(this, in state);
  }
}
