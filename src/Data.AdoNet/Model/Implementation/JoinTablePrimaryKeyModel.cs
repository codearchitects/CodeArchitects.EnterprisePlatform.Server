using CodeArchitects.Platform.Common.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class JoinTablePrimaryKeyModel : IPrimaryKeyModel
{
  private Type? _type;

  public JoinTablePrimaryKeyModel(IReadOnlyList<IPrimaryKeyColumnModel> columns)
  {
    Columns = columns;
  }

  public bool IsComposite => true;

  public Type Type
  {
    get
    {
      if (_type is not null)
        return _type;

      if (Columns.Count < 2)
        throw new InvalidOperationException("");

      _type = Columns.Count switch
      {
        2 => typeof(ValueTuple<,>),
        3 => typeof(ValueTuple<,,>),
        4 => typeof(ValueTuple<,,,>),
        5 => typeof(ValueTuple<,,,,>),
        6 => typeof(ValueTuple<,,,,,>),
        7 => typeof(ValueTuple<,,,,,,>),
        8 => typeof(ValueTuple<,,,,,,,>),
        _ => throw Errors.Unreachable
      };
      _type = _type.MakeGenericType(Columns.Map(column => column.Type));

      return _type;
    }
  }

  public IReadOnlyList<IPrimaryKeyColumnModel> Columns { get; }

  public Getter<object?> GetValue => throw new NotSupportedException("Getting the primary key value of a join table entity is not supported.");

  public Setter<object?> SetValue => throw new NotSupportedException("Setting the primary key value of a join table entity is not supported.");

  public bool TryGetColumn(ReadOnlySpan<char> name, [NotNullWhen(true)] out IPrimaryKeyColumnModel? column)
  {
    foreach (IPrimaryKeyColumnModel col in Columns)
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
}
