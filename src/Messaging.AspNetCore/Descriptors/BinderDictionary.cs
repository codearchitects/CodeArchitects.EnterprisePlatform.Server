using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Descriptors;

internal class BinderDictionary : IDictionary
{
  private readonly Dictionary<string, IConfigurationSection> _dictionary;
  private readonly IReadOnlyDictionary<string, PropertyInfo> _properties;

  public BinderDictionary(Dictionary<string, IConfigurationSection> dictionary, IReadOnlyDictionary<string, PropertyInfo> properties)
  {
    _dictionary = dictionary;
    _properties = properties;
  }

  public object? this[object key]
  {
    get
    {
      object? value = ((IDictionary)_dictionary)[key];
      if (value is not IConfigurationSection section)
        return null;

      if (key is not string stringKey)
        return section.Value;

      if (!_properties.TryGetValue(stringKey, out PropertyInfo? property))
        return section.Value;

      if (property.PropertyType == typeof(Type))
      {
        if (section.Value is not string stringValue)
          return null;

        return Type.GetType(stringValue);
      }

      if (property.PropertyType.IsAssignableFrom(typeof(Type[])))
      {
        if (section.Value is not null)
          return null;

        List<string> stringValues = new();
        section.Bind(stringValues);

        return stringValues.Select(val => Type.GetType(val)).ToArray();
      }

      return section.Value;
    }
    set => throw NotSupported();
  }

  private static Exception NotSupported() => new NotSupportedException("The dictionary is read-only.");

  #region Not supported

  public object SyncRoot => throw NotSupported();

  public void Add(object key, object? value) => throw NotSupported();

  public void Clear() => throw NotSupported();

  public void Remove(object key) => throw NotSupported();

  #endregion

  #region Implemented through _dictionary

  public bool IsFixedSize => true;

  public bool IsReadOnly => true;

  public ICollection Keys => ((IDictionary)_dictionary).Keys;

  public ICollection Values => ((IDictionary)_dictionary).Values;

  public int Count => _dictionary.Count;

  public bool IsSynchronized => true;

  public bool Contains(object key) => ((IDictionary)_dictionary).Contains(key);

  public void CopyTo(Array array, int index) => ((ICollection)_dictionary).CopyTo(array, index);

  public IDictionaryEnumerator GetEnumerator() => ((IDictionary)_dictionary).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();

  #endregion
}
