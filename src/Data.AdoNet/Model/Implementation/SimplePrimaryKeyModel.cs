namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class SimplePrimaryKeyModel<TKey> : PrimaryKeyModel<TKey>
  where TKey : IEquatable<TKey>
{
  public SimplePrimaryKeyModel(Getter<TKey> getValue, Setter<TKey> setValue)
  {
    GetValue = getValue;
    SetValue = setValue;
  }

  public override Getter<TKey> GetValue { get; }

  public override Setter<TKey> SetValue { get; }

  public override bool IsComposite => false;

  public override object GetKeyComponent(TKey key, int index)
  {
    if (index != 0)
      throw new ArgumentOutOfRangeException(nameof(index));

    return key;
  }
}
