using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Features.Concurrency;

namespace CodeArchitects.Platform.Data.AdoNet.Features.Concurrency;

internal class ConcurrencyContext : IConcurrencyContext
{
  private readonly IConcurrencyTokenProvider _tokenProvider;
  private readonly Dictionary<object, ConcurrencyEntry> _entries;

  public ConcurrencyContext(IConcurrencyTokenProvider tokenProvider)
  {
    _tokenProvider = tokenProvider;
    _entries = new();
  }

  public void CreateToken(object entity, IAccessibleColumnModel concurrencyColumn)
  {
    object? previousToken = concurrencyColumn.GetValue(entity);
    object token = _tokenProvider.CreateToken(concurrencyColumn.Type, previousToken);
    _entries[entity] = new(concurrencyColumn, token);
  }

  public object GetToken(object entity)
  {
    return _entries[entity].Token;
  }

  public void Accept()
  {
    foreach (KeyValuePair<object, ConcurrencyEntry> pair in _entries)
    {
      object entity = pair.Key;
      (IAccessibleColumnModel concurrencyColumn, object token) = pair.Value;

      concurrencyColumn.SetValue(entity, token);
    }
  }

  public void Clear()
  {
    _entries.Clear();
  }

  private readonly record struct ConcurrencyEntry(IAccessibleColumnModel ConcurrencyColumn, object Token);
}
