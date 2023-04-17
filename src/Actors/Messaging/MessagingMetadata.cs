using System.Reflection;

namespace CodeArchitects.Platform.Actors.Messaging;

internal abstract class MessagingMetadata
{
  public const string MessagingAssemblyName = "CodeArchitects.Platform.Messaging";
  public const string MessagingNamespace = "CodeArchitects.Platform.Messaging";

  public static readonly MessagingMetadata Metadata;

  static MessagingMetadata()
  {
    Assembly? messagingAssembly = null;

    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      if (assembly.GetName().Name != MessagingAssemblyName)
        continue;

      messagingAssembly = assembly;
      break;
    }

    Metadata = messagingAssembly is null
      ? UnresolvedMessagingMetadata.Instance
      : ResolvedMessagingMetadata.Create(messagingAssembly);
  }

  public abstract bool IsReferenced { get; }
  public abstract Type AttributeType { get; }
  public abstract ConstructorInfo AttributeConstructor { get; }
  public abstract PropertyInfo AttributeBusProperty { get; }
  public abstract PropertyInfo AttributeTopicProperty { get; }
  public abstract Type MessageHandlerType { get; }
  public abstract Type MessageHandlerWithResultType { get; }

  public abstract bool IsMessageHandlerType(Type type);
  public abstract bool IsMessageHandlerAttribute(Attribute attribute);

  private class ResolvedMessagingMetadata : MessagingMetadata
  {
    private ResolvedMessagingMetadata(Type attributeType, ConstructorInfo attributeConstructor, PropertyInfo attributeBusProperty, PropertyInfo attributeTopicProperty, Type messageHandlerType, Type messageHandlerWithResultType)
    {
      AttributeType = attributeType;
      AttributeConstructor = attributeConstructor;
      AttributeBusProperty = attributeBusProperty;
      AttributeTopicProperty = attributeTopicProperty;
      MessageHandlerType = messageHandlerType;
      MessageHandlerWithResultType = messageHandlerWithResultType;
    }

    public override bool IsReferenced => true;

    public override Type AttributeType { get; }

    public override ConstructorInfo AttributeConstructor { get; }

    public override PropertyInfo AttributeBusProperty { get; }

    public override PropertyInfo AttributeTopicProperty { get; }

    public override Type MessageHandlerType { get; }

    public override Type MessageHandlerWithResultType { get; }

    public static ResolvedMessagingMetadata Create(Assembly messagingAssembly)
    {
      Type? attributeType = messagingAssembly.GetType($"{MessagingNamespace}.MessageHandlerAttribute", throwOnError: true);

      ConstructorInfo constructor = attributeType.GetRequiredConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: new[] { typeof(string), typeof(string) });

      PropertyInfo busProperty = attributeType.GetRequiredProperty(
        name: "Bus",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public);

      PropertyInfo topicProperty = attributeType.GetRequiredProperty(
        name: "Topic",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public);

      Type messageHandlerType = messagingAssembly.GetType($"{MessagingNamespace}.IMessageHandler`1", throwOnError: true);
      Type messageHandlerWithResultType = messagingAssembly.GetType($"{MessagingNamespace}.IMessageHandler`2", throwOnError: true);

      return new(attributeType, constructor, busProperty, topicProperty, messageHandlerType, messageHandlerWithResultType);
    }

    public override bool IsMessageHandlerAttribute(Attribute attribute)
    {
      return attribute.GetType() == AttributeType;
    }

    public override bool IsMessageHandlerType(Type type)
    {
      if (!type.IsGenericType)
        return false;

      Type definition = type.GetGenericTypeDefinition();

      return definition == MessageHandlerType || definition == MessageHandlerWithResultType;
    }
  }

  private class UnresolvedMessagingMetadata : MessagingMetadata
  {
    public static readonly UnresolvedMessagingMetadata Instance = new();

    private UnresolvedMessagingMetadata() { }

    public override bool IsReferenced => false;

    public override Type AttributeType => throw UnresolvedException();

    public override ConstructorInfo AttributeConstructor => throw UnresolvedException();

    public override PropertyInfo AttributeBusProperty => throw UnresolvedException();

    public override PropertyInfo AttributeTopicProperty => throw UnresolvedException();

    public override Type MessageHandlerType => throw UnresolvedException();

    public override Type MessageHandlerWithResultType => throw UnresolvedException();

    public override bool IsMessageHandlerAttribute(Attribute attribute)
    {
      return false;
    }

    public override bool IsMessageHandlerType(Type type)
    {
      return false;
    }

    private Exception UnresolvedException() => new InvalidOperationException($"{MessagingAssemblyName} not referenced.");
  }
}
