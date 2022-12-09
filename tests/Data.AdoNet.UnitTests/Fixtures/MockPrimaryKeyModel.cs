using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Fixtures;

internal class MockPrimaryKeyModel<TKey> : IPrimaryKeyModel<TKey>
  where TKey : IEquatable<TKey>
{
  private readonly IPrimaryKeyModel _mock;

  public MockPrimaryKeyModel(IPrimaryKeyModel mock)
  {
    _mock = mock;
  }

  public bool IsComposite => _mock.IsComposite;

  public Type Type => _mock.Type;

  public IReadOnlyList<IPrimaryKeyColumnModel> Columns => _mock.Columns;

  public Getter<TKey> GetValue => instance => (TKey)_mock.GetValue(instance)!;

  public Setter<TKey> SetValue => (instance, value) => _mock.SetValue(instance, value);

  Getter<object?> IPrimaryKeyModel.GetValue => _mock.GetValue;

  Setter<object?> IPrimaryKeyModel.SetValue => _mock.SetValue;

  public object GetKeyComponent(TKey key, int index)
  {
    if (key is ITuple tuple)
      return tuple[index]!;

    return key;
  }

  public bool TryGetColumn(ReadOnlySpan<char> name, [NotNullWhen(true)] out IPrimaryKeyColumnModel? column)
  {
    throw new NotImplementedException();
  }
}

internal static class MockPrimaryKeyPropertyModelExtensions
{
  public static IPrimaryKeyColumnModel Mocked(this IPrimaryKeyColumnModel mock)
  {
    return new MockPrimaryKeyPropertyModel(mock);
  }
}
