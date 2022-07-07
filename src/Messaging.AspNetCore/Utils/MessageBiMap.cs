using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Utils;

/// <summary>
/// Implementation of <see cref="IMessageBiMap"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal class MessageBiMap : IMessageBiMap
{
  private readonly Dictionary<string, Type> _nameToType;
  private readonly Dictionary<Type, string> _typeToName;

  /// <summary>
  /// Creates a new <see cref="MessageBiMap"/> instance.
  /// </summary>
  public MessageBiMap()
  {
    _nameToType = new();
    _typeToName = new();
  }

  /// <summary>
  /// Configures the mapping between the type and name of a message.
  /// </summary>
  /// <param name="messageType">The type of the message.</param>
  /// <param name="messageName">The name of the message.</param>
  public void Add(Type messageType, string messageName)
  {
    _nameToType.Add(messageName, messageType);
    _typeToName.Add(messageType, messageName);
  }

  Type IReadOnlyDictionary<string, Type>.this[string key] => _nameToType[key];

  string IReadOnlyDictionary<Type, string>.this[Type key] => _typeToName[key];

  IEnumerable<string> IReadOnlyDictionary<string, Type>.Keys => _nameToType.Keys;

  IEnumerable<Type> IReadOnlyDictionary<Type, string>.Keys => _typeToName.Keys;

  IEnumerable<Type> IReadOnlyDictionary<string, Type>.Values => _nameToType.Values;

  IEnumerable<string> IReadOnlyDictionary<Type, string>.Values => _typeToName.Values;

  int IReadOnlyCollection<KeyValuePair<string, Type>>.Count => _nameToType.Count;

  int IReadOnlyCollection<KeyValuePair<Type, string>>.Count => _typeToName.Count;

  bool IReadOnlyDictionary<string, Type>.ContainsKey(string key)
  {
    return _nameToType.ContainsKey(key);
  }

  bool IReadOnlyDictionary<Type, string>.ContainsKey(Type key)
  {
    return _typeToName.ContainsKey(key);
  }

  IEnumerator<KeyValuePair<string, Type>> IEnumerable<KeyValuePair<string, Type>>.GetEnumerator()
  {
    return _nameToType.GetEnumerator();
  }

  IEnumerator<KeyValuePair<Type, string>> IEnumerable<KeyValuePair<Type, string>>.GetEnumerator()
  {
    return _typeToName.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    throw new NotSupportedException();
  }

  bool IReadOnlyDictionary<string, Type>.TryGetValue(string key, [MaybeNullWhen(false)] out Type value)
  {
    return _nameToType.TryGetValue(key, out value);
  }

  bool IReadOnlyDictionary<Type, string>.TryGetValue(Type key, [MaybeNullWhen(false)] out string value)
  {
    return _typeToName.TryGetValue(key, out value);
  }
}
