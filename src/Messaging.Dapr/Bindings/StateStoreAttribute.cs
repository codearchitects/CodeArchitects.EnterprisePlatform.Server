namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)]
public class StateStoreAttribute : Attribute, IStateStoreOutputMetadata
{
  public StateStoreAttribute(string storeName, string key, params Type[] resultTypes)
  {
    StoreName = storeName;
    Key = key;
    AllowedTypes = resultTypes;
  }

  public string StoreName { get; }

  public string Key { get; }

  public IEnumerable<Type>? AllowedTypes { get; }
}
