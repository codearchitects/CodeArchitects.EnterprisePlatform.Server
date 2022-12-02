using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Fixtures;

internal class MockOrdinaryPropertyModel : IOrdinaryPropertyModel
{
  private readonly IOrdinaryPropertyModel _mock;

  public MockOrdinaryPropertyModel(IOrdinaryPropertyModel mock)
  {
    _mock = mock;
  }

  public bool IsPrimaryKey => _mock.IsPrimaryKey;

  public bool IsForeignKey => _mock.IsForeignKey;

  public string ColumnName => _mock.ColumnName;

  public short Index => _mock.Index;

  public MemberInfo Member => _mock.Member;

  public Getter<object?> GetValue => _mock.GetValue;

  public Setter<object?> SetValue => _mock.SetValue;

  public Type Type => _mock.Type;

  public FieldInfo? Field => _mock.Field;

  public PropertyInfo? Property => _mock.Property;

  public bool HasMember => _mock.HasMember;

  public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
    where TVisitor : IPropertyModelVisitor<TResult>
  {
    return visitor.VisitOrdinary(this);
  }

  public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
    where TVisitor : IPropertyModelVisitor<TResult, TState>
  {
    return visitor.VisitOrdinary(this, in state);
  }
}

internal static class MockOrdinaryPropertyModelExtensions
{
  public static IOrdinaryPropertyModel Mocked(this IOrdinaryPropertyModel mock)
  {
    return new MockOrdinaryPropertyModel(mock);
  }
}
