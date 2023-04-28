using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class JoinColumnModel : ColumnModel, IPrimaryKeyColumnModel
{
  private readonly JoinColumnMemberComponent _memberComponent;

  public JoinColumnModel(JoinColumnMemberComponent memberComponent, short index)
    : base(index)
  {
    _memberComponent = memberComponent;
  }

  public override bool IsPrimaryKey => true;

  public override bool IsForeignKey => true;

  public override bool IsConcurrencyToken
  {
    get => false;
    set => throw new InvalidOperationException("A join column cannot be a concurrency token.");
  }

  public override string Name
  {
    get => _memberComponent.Name;
    set => throw new InvalidOperationException("Cannot set the name of a join column.");
  }

  protected override MemberComponent<object?> MemberComponent => _memberComponent;

  public new MemberInfo Member => _memberComponent.Member;

  public new Getter<object?> GetValue => _memberComponent.GetValue;

  public new Setter<object?> SetValue => _memberComponent.SetValue;

  public override TResult Accept<TResult>(IColumnModelVisitor<TResult> visitor)
  {
    return visitor.VisitPrimaryKey(this);
  }

  public override TResult Accept<TResult, TState>(IColumnModelVisitor<TResult, TState> visitor, in TState state)
  {
    return visitor.VisitPrimaryKey(this, in state);
  }

  public static JoinColumnModel Create(string name, Type type, short index)
  {
    JoinColumnMemberComponent memberComponent = JoinColumnMemberComponent.Create(name, type);

    return new(memberComponent, index);
  }
}
