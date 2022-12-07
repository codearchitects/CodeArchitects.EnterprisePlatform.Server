using CodeArchitects.Platform.Data.AdoNet.Model;
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

  public IReadOnlyList<IStandardPrimaryKeyColumnModel> Columns => _mock.Columns;

  public ConstructorInfo? Constructor => _mock.Constructor;

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
}

internal static class MockPrimaryKeyPropertyModelExtensions
{
  public static IStandardPrimaryKeyColumnModel Mocked(this IStandardPrimaryKeyColumnModel mock)
  {
    return new MockPrimaryKeyPropertyModel(mock);
  }
}
