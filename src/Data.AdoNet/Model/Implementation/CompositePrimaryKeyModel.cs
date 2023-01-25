namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class CompositePrimaryKeyModel<TKey1, TKey2> : PrimaryKeyModel<(TKey1, TKey2)>
{
  public CompositePrimaryKeyModel(Getter<(TKey1, TKey2)> getValue, Setter<(TKey1, TKey2)> setValue)
  {
    GetValue = getValue;
    SetValue = setValue;
  }

  public override Getter<(TKey1, TKey2)> GetValue { get; }

  public override Setter<(TKey1, TKey2)> SetValue { get; }

  public override bool IsComposite => true;

  public override object? GetKeyComponent((TKey1, TKey2) key, int index)
  {
    return index switch
    {
      0 => key.Item1,
      1 => key.Item2,
      _ => throw new ArgumentOutOfRangeException(nameof(index))
    };
  }
}

internal class CompositePrimaryKeyModel<TKey1, TKey2, TKey3> : PrimaryKeyModel<(TKey1, TKey2, TKey3)>
{
  public CompositePrimaryKeyModel(Getter<(TKey1, TKey2, TKey3)> getValue, Setter<(TKey1, TKey2, TKey3)> setValue)
  {
    GetValue = getValue;
    SetValue = setValue;
  }

  public override Getter<(TKey1, TKey2, TKey3)> GetValue { get; }

  public override Setter<(TKey1, TKey2, TKey3)> SetValue { get; }

  public override bool IsComposite => true;

  public override object? GetKeyComponent((TKey1, TKey2, TKey3) key, int index)
  {
    return index switch
    {
      0 => key.Item1,
      1 => key.Item2,
      2 => key.Item3,
      _ => throw new ArgumentOutOfRangeException(nameof(index))
    };
  }
}

internal class CompositePrimaryKeyModel<TKey1, TKey2, TKey3, TKey4> : PrimaryKeyModel<(TKey1, TKey2, TKey3, TKey4)>
{
  public CompositePrimaryKeyModel(Getter<(TKey1, TKey2, TKey3, TKey4)> getValue, Setter<(TKey1, TKey2, TKey3, TKey4)> setValue)
  {
    GetValue = getValue;
    SetValue = setValue;
  }

  public override Getter<(TKey1, TKey2, TKey3, TKey4)> GetValue { get; }

  public override Setter<(TKey1, TKey2, TKey3, TKey4)> SetValue { get; }

  public override bool IsComposite => true;

  public override object? GetKeyComponent((TKey1, TKey2, TKey3, TKey4) key, int index)
  {
    return index switch
    {
      0 => key.Item1,
      1 => key.Item2,
      2 => key.Item3,
      3 => key.Item4,
      _ => throw new ArgumentOutOfRangeException(nameof(index))
    };
  }
}
