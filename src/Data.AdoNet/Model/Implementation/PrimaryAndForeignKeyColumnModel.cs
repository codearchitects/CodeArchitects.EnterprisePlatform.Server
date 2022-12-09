using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class PrimaryAndForeignKeyColumnModel : ColumnModel, IPrimaryAndForeignKeyColumnModel
{
  private readonly AccessibleMemberComponent<object?> _memberComponent;

  public PrimaryAndForeignKeyColumnModel(AccessibleMemberComponent<object?> memberComponent, short index, short primaryKeyIndex, short foreignKeyIndex, INavigationModel navigation)
    : base(index)
  {
    _memberComponent = memberComponent;
    PrimaryKeyIndex = primaryKeyIndex;
    ForeignKeyIndex = foreignKeyIndex;
    Navigation = navigation;
  }

  protected override MemberComponent<object?> MemberComponent => _memberComponent;

  public override bool IsPrimaryKey => true;

  public override bool IsForeignKey => true;

  public short PrimaryKeyIndex { get; }

  public short ForeignKeyIndex { get; }

  public IPrimaryKeyColumnModel PrimaryKeyColumn => Navigation.PrimaryKey.Columns[ForeignKeyIndex];

  public INavigationModel Navigation { get; }

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
    return visitor.VisitPrimaryAndForeignKey(this);
  }

  public override TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
  {
    return visitor.VisitPrimaryAndForeignKey(this, in state);
  }
}
