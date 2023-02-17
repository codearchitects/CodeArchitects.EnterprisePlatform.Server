using CodeArchitects.Platform.Actors.Descriptors.Implementation;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Common.Exceptions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal class MethodDescriptorFactory
{
  private readonly IActivityTypeBuilder _activityTypeBuilder;
  private readonly Type _actorType;
  private readonly Type _activityBaseType;
  private readonly Dictionary<MethodInfo, IMethodDescriptor> _methods;
  private readonly Dictionary<MethodInfo, IMethodDescriptor> _activities;
  private int _id;

  private MethodDescriptorFactory(IActivityTypeBuilder activityTypeBuilder, Type actorType, Type activityBaseType)
  {
    _activityTypeBuilder = activityTypeBuilder;
    _actorType = actorType;
    _activityBaseType = activityBaseType;
    _methods = new();
    _activities = new();
    _id = 1;
  }

  public IReadOnlyCollection<IMethodDescriptor> Methods => _methods.Values;

  public IReadOnlyCollection<IMethodDescriptor> Activities => _activities.Values;

  public void AddImplementationType(Type implementationType)
  {
    MethodInfo[] methods = implementationType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
    foreach (MethodInfo method in methods)
    {
      AddActivity(method);
    }
  }

  private void AddMethod(MethodInfo interfaceMethod, MethodInfo implementationMethod)
  {
    if (!TryGetKind(implementationMethod, out MethodKind kind) || kind is MethodKind.Void)
      throw InvalidActorException.InvalidMethodReturnType(implementationMethod.DeclaringType, implementationMethod);

    MethodDescriptor descriptor = CreateDescriptor(kind, interfaceMethod, implementationMethod);

    _methods.Add(implementationMethod, descriptor);
    _activities.Add(implementationMethod, descriptor);
  }

  private void AddActivity(MethodInfo implementationMethod)
  {
    if (_activities.ContainsKey(implementationMethod.GetBaseDefinition()))
      return;

    if (!TryGetKind(implementationMethod, out MethodKind kind))
      return;

    MethodDescriptor descriptor = CreateDescriptor(kind, null, implementationMethod);
    _activities.Add(implementationMethod, descriptor);
  }

  private MethodDescriptor CreateDescriptor(MethodKind kind, MethodInfo? interfaceMethod, MethodInfo implementationMethod)
  {
    if (implementationMethod.IsGenericMethod)
      throw InvalidActorException.GenericMethodsAreNotSupported(_actorType, implementationMethod);

    Type[] parameterTypes = GetParameterTypes(implementationMethod);
    Func<IMethodDescriptor, Type> activityTypeFactory = descriptor => _activityTypeBuilder.Build(descriptor, _actorType, _activityBaseType);

    MethodDescriptor descriptor = kind switch
    {
      MethodKind.Void       => new VoidMethodDescriptor(_id, implementationMethod, parameterTypes, activityTypeFactory),
      MethodKind.Task       => new TaskMethodDescriptor(_id, interfaceMethod, implementationMethod, parameterTypes, activityTypeFactory),
      MethodKind.TaskT      => new TaskTMethodDescriptor(_id, interfaceMethod, implementationMethod, parameterTypes, activityTypeFactory),
      MethodKind.ValueTask  => new ValueTaskMethodDescriptor(_id, interfaceMethod, implementationMethod, parameterTypes, activityTypeFactory),
      MethodKind.ValueTaskT => new ValueTaskTMethodDescriptor(_id, interfaceMethod, implementationMethod, parameterTypes, activityTypeFactory),
      _                     => throw Errors.Unreachable
    };

    _id++;
    return descriptor;
  }

  private static bool TryGetKind(MethodInfo method, out MethodKind kind)
  {
    Type returnType = method.ReturnType;
    if (returnType.IsGenericType)
    {
      returnType = returnType.GetGenericTypeDefinition();
      if (returnType == typeof(Task<>))
      {
        kind = MethodKind.TaskT;
        return true;
      }

      if (returnType == typeof(ValueTask<>))
      {
        kind = MethodKind.ValueTaskT;
        return true;
      }
    }
    else
    {
      if (returnType == typeof(Task))
      {
        kind = MethodKind.Task;
        return true;
      }

      if (returnType == typeof(ValueTask))
      {
        kind = MethodKind.ValueTask;
        return true;
      }

      if (returnType == typeof(void))
      {
        kind = MethodKind.Void;
        return true;
      }
    }

    kind = default;
    return false;
  }

  private Type[] GetParameterTypes(MethodInfo method)
  {
    ParameterInfo[] parameters = method.GetParameters();
    Type[] parameterTypes = new Type[parameters.Length];

    for (int i = 0; i < parameters.Length; i++)
    {
      ParameterInfo parameter = parameters[i];
      if (i != parameters.Length - 1 && parameter.ParameterType == typeof(CancellationToken))
        throw InvalidActorException.CancellationTokenMustBeLastParameter(method.DeclaringType, method);

      parameterTypes[i] = parameter.ParameterType;
    }

    return parameterTypes;
  }

  public static MethodDescriptorFactory Create(IActivityTypeBuilder activityTypeBuilder, Type actorType, Type interfaceType, Type activityBaseType)
  {
    MethodDescriptorFactory factory = new(activityTypeBuilder, actorType, activityBaseType);

    InterfaceMapping mapping = actorType.GetInterfaceMap(interfaceType);

    for (int i = 0; i < mapping.TargetMethods.Length; i++)
    {
      MethodInfo interfaceMethods = mapping.InterfaceMethods[i];
      MethodInfo implementationMethod = mapping.TargetMethods[i];

      factory.AddMethod(interfaceMethods, implementationMethod);
    }

    factory.AddImplementationType(actorType);

    return factory;
  }
}
