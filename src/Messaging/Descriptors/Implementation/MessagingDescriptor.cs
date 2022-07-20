using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

/// <summary>
/// Implementation of <see cref="IMessagingDescriptor"/>
/// </summary>
internal record MessagingDescriptor(
  IEnumerable<IHandlerDescriptor> HandlerDescriptors,
  IEnumerable<IMessageDescriptor> MessageDescriptors) : IMessagingDescriptor
{
  public static IMessagingDescriptor Create(IEnumerable<Type> concreteTypes, IEnumerable<Type> messageTypes, string? defaultBus, string? defaultTopic, ICollection<HandlerDiagnostics> diagnosticCollection)
  {
    List<IHandlerDescriptor> handlerDescriptors = new();
    Dictionary<Type, IMessageDescriptor> messageDescriptors = new();

    foreach (Type messageType in messageTypes)
    {
      TryAddMessageDescriptor(messageType);
    }

    foreach (Type concreteType in concreteTypes)
    {
      foreach (HandlerDescriptor handlerDescriptor in HandlerDescriptor.Create(concreteType, defaultBus, defaultTopic, diagnosticCollection))
      {
        handlerDescriptors.Add(handlerDescriptor);
        TryAddMessageDescriptor(handlerDescriptor.MessageType);
      }
    }

    return new MessagingDescriptor(handlerDescriptors, messageDescriptors.Values);

    void TryAddMessageDescriptor(Type messageType)
    {
      bool isAncestor = false;
      while (messageType is not null)
      {
        if (!messageDescriptors.ContainsKey(messageType))
        {
          if (isAncestor && !messageType.IsDefined(typeof(MessageAttribute)))
            return;

          messageDescriptors.Add(messageType, MessageDescriptor.Create(messageType));
        }

        messageType = messageType.BaseType!;
        isAncestor = true;
      }
    }
  }

  public static IMessagingDescriptor Merge(IMessagingDescriptor first, IMessagingDescriptor second, ICollection<HandlerDiagnostics> diagnosticCollection)
  {
    Dictionary<Type, IMessageDescriptor> messageDescriptors = first.MessageDescriptors.ToDictionary(descr => descr.Type);
    foreach (IMessageDescriptor messageDescriptor in second.MessageDescriptors)
    {
      if (!messageDescriptors.ContainsKey(messageDescriptor.Type))
      {
        messageDescriptors.Add(messageDescriptor.Type, messageDescriptor);
      }
    }

    HashSet<IHandlerDescriptor> checkedDescriptors = new();
    IEnumerable<IHandlerDescriptor> allHandlerDescriptors = first.HandlerDescriptors.Concat(second.HandlerDescriptors);
    List<IHandlerDescriptor> handlerDescriptors = new();

    foreach (IHandlerDescriptor handlerDescriptor in allHandlerDescriptors)
    {
      if (checkedDescriptors.Contains(handlerDescriptor))
        continue;

      if (allHandlerDescriptors.Any(descr => IsSameHandler(handlerDescriptor, descr, checkedDescriptors)))
      {
        Dictionary<Type, IOutputBindingDescriptor> outputBindingDescriptors = handlerDescriptor.OutputBindingDescriptors.ToDictionary(descr => descr.MetadataType);
        foreach (IHandlerDescriptor thisHandlerDescriptor in allHandlerDescriptors)
        {
          if (!IsSameHandler(handlerDescriptor, thisHandlerDescriptor, checkedDescriptors))
            continue;

          checkedDescriptors.Add(thisHandlerDescriptor);

          foreach (IOutputBindingDescriptor outputBindingDescriptor in thisHandlerDescriptor.OutputBindingDescriptors)
          {
            if (outputBindingDescriptors.ContainsKey(outputBindingDescriptor.MetadataType))
            {
              diagnosticCollection.Add(DuplicateOutputBinding(handlerDescriptor.ConcreteType));
              continue;
            }

            outputBindingDescriptors.Add(outputBindingDescriptor.MetadataType, outputBindingDescriptor);
          }
        }

        handlerDescriptors.Add(new HandlerDescriptor(handlerDescriptor.Bus, handlerDescriptor.Topic, handlerDescriptor.InterfaceType, handlerDescriptor.ConcreteType, outputBindingDescriptors.Values));
      }
      else
      {
        handlerDescriptors.Add(handlerDescriptor);
      }
    }

    return new MessagingDescriptor(handlerDescriptors, messageDescriptors.Values);

    static bool IsSameHandler(IHandlerDescriptor first, IHandlerDescriptor second, HashSet<IHandlerDescriptor> checkedDescriptors)
    {
      return
        first != second &&
        !checkedDescriptors.Contains(second) &&
        first.ConcreteType == second.ConcreteType &&
        first.InterfaceType == second.InterfaceType &&
        first.Bus == second.Bus &&
        first.Topic == second.Topic;
    }
  }

  private static HandlerDiagnostics DuplicateOutputBinding(Type concreteType)
    => new(concreteType, "Duplicate output binding of the same metadata found on handler {0}.", concreteType);
}
