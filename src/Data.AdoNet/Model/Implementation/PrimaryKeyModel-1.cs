namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class PrimaryKeyModel<TKey> : PrimaryKeyModel, IPrimaryKeyModel<TKey>
  where TKey : IEquatable<TKey>
{
  private Getter<object?>? _getValueUntyped;
  private Setter<object?>? _setValueUntyped;

  public new abstract Getter<TKey> GetValue { get; }

  public new abstract Setter<TKey> SetValue { get; }

  public abstract object? GetKeyComponent(TKey key, int index);

  protected override Getter<object?> GetValueCore => _getValueUntyped ??= instance => GetValue(instance);

  protected override Setter<object?> SetValueCore => _setValueUntyped ??= (instance, value) => SetValue(instance, (TKey)value!);

  public override Type Type => typeof(TKey);
}
