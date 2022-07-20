namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

[AttributeUsage(AttributeTargets.ReturnValue)]
public class StateStoreAttribute : Attribute, IStateStoreOutputMetadata
{
  public StateStoreAttribute(string storeName, string key)
  {
    StoreName = storeName;
    Key = key;
  }

  public string StoreName { get; }

  public string Key { get; }
}
