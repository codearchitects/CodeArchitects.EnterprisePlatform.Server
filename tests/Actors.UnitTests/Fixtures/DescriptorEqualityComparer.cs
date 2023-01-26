using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Common.Utils;

namespace CodeArchitects.Platform.Actors.Fixtures;

internal class DescriptorEqualityComparer :
  IEqualityComparer<IActorDescriptor>,
  IEqualityComparer<IImplementationDescriptor>,
  IEqualityComparer<IMethodDescriptor>,
  IEqualityComparer<IConstructorDescriptor>,
  IEqualityComparer<IDependencyDescriptor>,
  IEqualityComparer<IActorIdDescriptor>,
  IEqualityComparer<IStateDescriptor>,
  IEqualityComparer<IActorFactoryDescriptor>
{
  public static readonly DescriptorEqualityComparer Instance = new DescriptorEqualityComparer();

  private DescriptorEqualityComparer() { }

  public bool Equals(IActorDescriptor? x, IActorDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.InterfaceType != y.InterfaceType)
      return false;

    if (x.ActorType != y.ActorType)
      return false;

    if (x.IsPolymorphic != y.IsPolymorphic)
      return false;

    if (!Equals(x.BaseImplementation, y.BaseImplementation))
      return false;

    if (!Equals(x.DefaultImplementation, y.DefaultImplementation))
      return false;

    if (!x.Implementations.SequenceEqual(y.Implementations, this))
      return false;

    if (!Equals(x.Id, y.Id))
      return false;

    if (!Equals(x.State, y.State))
      return false;

    if (!Equals(x.Factory, y.Factory))
      return false;

    return true;
  }

  public bool Equals(IImplementationDescriptor? x, IImplementationDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Type != y.Type)
      return false;

    if (!Equals(x.Constructor, y.Constructor))
      return false;

    if (!x.Methods.SequenceEqual(y.Methods, this))
      return false;

    return true;
  }

  public bool Equals(IConstructorDescriptor? x, IConstructorDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Constructor != y.Constructor)
      return false;

    if (!x.Dependencies.SequenceEqual(y.Dependencies, this))
      return false;

    if (!Equals(x.ContextDependency, y.ContextDependency))
      return false;

    if (!x.ServiceDependencies.SequenceEqual(y.ServiceDependencies, this))
      return false;

    if (!x.StateDependencies.SequenceEqual(y.StateDependencies, this))
      return false;

    return true;
  }

  public bool Equals(IActorIdDescriptor? x, IActorIdDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.IdType != y.IdType)
      return false;

    if (x.HasIdSource != y.HasIdSource)
      return false;

    if (!Equals(x.StateDependency, y.StateDependency))
      return false;

    if (x.StateProperty != y.StateProperty)
      return false;

    return true;
  }

  public bool Equals(IStateDescriptor? x, IStateDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.StateType != y.StateType)
      return false;

    if (x.IsStateless != y.IsStateless)
      return false;

    if (x.IsVirtual != y.IsVirtual)
      return false;

    if (!x.Fields.SequenceEqual(y.Fields))
      return false;

    if (x.DefaultValues is not null)
    {
      if (y.DefaultValues is null)
        return false;

      if (!x.DefaultValues.SequenceEqual(y.DefaultValues))
        return false;
    }
    else if (y.DefaultValues is not null)
      return false;

    return true;
  }

  public bool Equals(IActorFactoryDescriptor? x, IActorFactoryDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.FactoryType != y.FactoryType)
      return false;

    if (x.CreateAsyncMethod != y.CreateAsyncMethod)
      return false;

    if (x.GetMethod != y.GetMethod)
      return false;

    return true;
  }

  public bool Equals(IMethodDescriptor? x, IMethodDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Kind != y.Kind)
      return false;

    if (x.InterfaceMethod != y.InterfaceMethod)
      return false;

    if (x.ImplementationMethod != y.ImplementationMethod)
      return false;

    if (!x.ParameterTypes.SequenceEqual(y.ParameterTypes))
      return false;
    
    if (x.IsStateless != y.IsStateless)
      return false;

    if (x.HasCancellationTokenParameter != y.HasCancellationTokenParameter)
      return false;

    if (x.CancellationTokenParameterPosition != y.CancellationTokenParameterPosition)
      return false;

    return x switch
    {
      IVoidMethodDescriptor                   => y is IVoidMethodDescriptor,
      ITaskMethodDescriptor                   => y is ITaskMethodDescriptor,
      IValueTaskMethodDescriptor              => y is IValueTaskMethodDescriptor,
      ITaskTMethodDescriptor xTaskT           => y is ITaskTMethodDescriptor yTaskT && SpecificEquals(xTaskT, yTaskT),
      IValueTaskTMethodDescriptor xValueTaskT => y is IValueTaskTMethodDescriptor yValueTaskT && SpecificEquals(xValueTaskT, yValueTaskT),
      _                                       => throw Errors.Unreacheable,
    };
  }

  private static bool SpecificEquals(ITaskTMethodDescriptor x, ITaskTMethodDescriptor y)
  {
    return x.ResultType == y.ResultType;
  }

  private static bool SpecificEquals(IValueTaskTMethodDescriptor x, IValueTaskTMethodDescriptor y)
  {
    return x.ResultType == y.ResultType;
  }

  public bool Equals(IDependencyDescriptor? x, IDependencyDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Kind != y.Kind)
      return false;

    if (x.Parameter != y.Parameter)
      return false;

    if (x.Name != y.Name)
      return false;

    if (x.Type != y.Type)
      return false;

    if (x.Index != y.Index)
      return false;

    if (x.CategoryIndex != y.CategoryIndex)
      return false;

    return x switch
    {
      IContextDependencyDescriptor          => y is IContextDependencyDescriptor,
      IServiceDependencyDescriptor xService => y is IServiceDependencyDescriptor yService && SpecificEquals(xService, yService),
      IStateDependencyDescriptor xState     => y is IStateDependencyDescriptor yState && SpecificEquals(xState, yState),
      _                                     => throw Errors.Unreacheable
    };
  }

  private static bool SpecificEquals(IServiceDependencyDescriptor x, IServiceDependencyDescriptor y)
  {
    return x.IsOptional == y.IsOptional;
  }

  private static bool SpecificEquals(IStateDependencyDescriptor x, IStateDependencyDescriptor y)
  {
    return x.Field == y.Field;
  }

  public int GetHashCode(IActorDescriptor obj)
  {
    return 0;
  }

  public int GetHashCode(IImplementationDescriptor obj)
  {
    return 0;
  }

  public int GetHashCode(IConstructorDescriptor obj)
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

  public int GetHashCode(IMethodDescriptor obj)
  {
    return 0;
  }

  public int GetHashCode(IDependencyDescriptor obj)
  {
    return 0;
  }
}
