namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

/// <summary>
/// Indicates that the result of a handler method should be saved into a state store.
/// </summary>
[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)]
public class StateStoreAttribute : Attribute, IStateStoreOutputMetadata
{
  /// <summary>
  /// Creates a new <see cref="StateStoreAttribute"/>, specifying the store name and the key.
  /// </summary>
  /// <param name="store">The name of the store to save the result into.</param>
  /// <param name="key">The state key.</param>
  public StateStoreAttribute(string store, string key)
  {
    Store = store;
    Key = key;
  }

  /// <summary>
  /// Creates a new <see cref="StateStoreAttribute"/>, specifying the store name, the key and the set of result types that will be subject to the output binding.
  /// </summary>
  /// <param name="store">The name of the store to save the result into.</param>
  /// <param name="key">The state key.</param>
  /// <param name="resultTypes">The set of result types that will be subject to the output binding.</param>
  public StateStoreAttribute(string store, string key, params Type[] resultTypes)
  {
    Store = store;
    Key = key;
    AllowedTypes = resultTypes;
  }

  /// <summary>
  /// The name of the store to save the result into.
  /// </summary>
  public string Store { get; }

  /// <summary>
  /// The state key.
  /// </summary>
  public string Key { get; }

  /// <summary>
  /// The set of result types that will be subject to the output binding.
  /// </summary>
  public IEnumerable<Type>? AllowedTypes { get; }
}
