using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Common.Exceptions;

namespace CodeArchitects.Platform.Actors.TestModel;

internal class ActorDescriptorEqualityComparer :
  IEqualityComparer<IActorDescriptor>,
  IEqualityComparer<IImplementationDescriptor>,
  IEqualityComparer<IMethodDescriptor>,
  IEqualityComparer<IActorIdDescriptor>,
  IEqualityComparer<IStateDescriptor>,
  IEqualityComparer<IActorFactoryDescriptor>
{
  public static readonly ActorDescriptorEqualityComparer Instance = new ActorDescriptorEqualityComparer();

  public bool Equals(IActorDescriptor? x, IActorDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.ActorType.Equals(y.ActorType))
      return false;

    if (!x.InterfaceType.Equals(y.InterfaceType))
      return false;

    if (!x.ActivityBaseType.Equals(y.ActivityBaseType))
      return false;

    if (x.IsPolymorphic != y.IsPolymorphic)
      return false;

    if (x.IsVirtual != y.IsVirtual)
      return false;

    if (!Equals(x.BaseImplementation, y.BaseImplementation))
      return false;

    if (!Equals(x.DefaultImplementation, y.DefaultImplementation))
      return false;

    if (!x.Implementations.SequenceEqual(y.Implementations, this))
      return false;

    if (!x.Methods.SequenceEqual(y.Methods, this))
      return false;

    if (!x.Activities.SequenceEqual(y.Activities, this))
      return false;

    if (!Equals(x.Id, y.Id))
      return false;

    if (!Equals(x.State, y.State))
      return false;

    return true;
  }

  public bool Equals(IImplementationDescriptor? x, IImplementationDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Id != y.Id)
      return false;

    if (!x.Type.Equals(y.Type))
      return false;

    return true;
  }

  public bool Equals(IMethodDescriptor? x, IMethodDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Id != y.Id)
      return false;

    if (x.Kind != y.Kind)
      return false;

    if (x.InterfaceMethod != y.InterfaceMethod)
      return false;

    if (x.ImplementationMethod != y.ImplementationMethod)
      return false;

    if (!x.ReturnType.Equals(y.ReturnType))
      return false;

    if (!x.ParameterTypes.SequenceEqual(y.ParameterTypes))
      return false;

    if (!x.ActivityType.Equals(y.ActivityType))
      return false;

    if (!x.ActivityFields.SequenceEqual(y.ActivityFields))
      return false;

    if (x.HasCancellationTokenParameter != y.HasCancellationTokenParameter)
      return false;

    return x switch
    {
      IVoidMethodDescriptor                   => y is IVoidMethodDescriptor,
      ITaskMethodDescriptor                   => y is ITaskMethodDescriptor,
      IValueTaskMethodDescriptor              => y is IValueTaskMethodDescriptor,
      ITaskTMethodDescriptor xTaskT           => y is ITaskTMethodDescriptor yTaskT && Equals(xTaskT, yTaskT),
      IValueTaskTMethodDescriptor xValueTaskT => y is IValueTaskTMethodDescriptor yValueTaskT && Equals(xValueTaskT, yValueTaskT),
      _                                       => throw Errors.Unreachable
    };
  }

  private static bool Equals(ITaskTMethodDescriptor x, ITaskTMethodDescriptor y)
  {
    return x.ResultType.Equals(y.ResultType);
  }

  private static bool Equals(IValueTaskTMethodDescriptor x, IValueTaskTMethodDescriptor y)
  {
    return x.ResultType.Equals(y.ResultType);
  }

  public bool Equals(IActorIdDescriptor? x, IActorIdDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Type.Equals(y.Type))
      return false;

    if (x.HasIdSource != y.HasIdSource)
      return false;

    if (x.StateIndex != y.StateIndex)
      return false;

    if (x.GetActorIdMethod != y.GetActorIdMethod)
      return false;

    return true;
  }

  public bool Equals(IStateDescriptor? x, IStateDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Type.Equals(y.Type))
      return false;

    if (!x.Fields.SequenceEqual(y.Fields))
      return false;

    if (x.DefaultValue is { } defaultValue)
    {
      if (!Equals(defaultValue, y.DefaultValue))
        return false;
    }
    else
    {
      if (y.DefaultValue != null)
        return false;
    }

    return true;
  }

  public bool Equals(IActorFactoryDescriptor? x, IActorFactoryDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.FactoryType.Equals(y.FactoryType))
      return false;

    if (x.CreateAsyncMethod != y.CreateAsyncMethod)
      return false;

    if (x.GetMethod != y.GetMethod)
      return false;

    return true;
  }

  public int GetHashCode(IActorDescriptor obj)
  {
    return 0;
  }

  public int GetHashCode(IImplementationDescriptor obj)
  {
    return 0;
  }

  public int GetHashCode(IMethodDescriptor obj)
  {
    return 0;
  }

  public int GetHashCode(IActorIdDescriptor obj)
  {
    return 0;
  }

  public int GetHashCode(IStateDescriptor obj)
  {
    return 0;
  }

  public int GetHashCode(IActorFactoryDescriptor obj)
  {
    return 0;
  }
}
