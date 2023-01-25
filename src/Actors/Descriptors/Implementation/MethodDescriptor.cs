using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Common.Utils;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal abstract class MethodDescriptor : IMethodDescriptor
{
  private IReadOnlyList<Type>? _parameterTypes;

  protected MethodDescriptor(MethodInfo interfaceMethod, MethodInfo implementationMethod, bool isStateless, int cancellationTokenParameterPosition)
  {
    InterfaceMethod = interfaceMethod;
    ImplementationMethod = implementationMethod;
    IsStateless = isStateless;
    CancellationTokenParameterPosition = cancellationTokenParameterPosition;
  }

  public abstract MethodKind Kind { get; }

  public MethodInfo InterfaceMethod { get; }

  public MethodInfo ImplementationMethod { get; }

  public IReadOnlyList<Type> ParameterTypes => _parameterTypes ??= GetParameterTypes();

  public bool IsStateless { get; }

  public int CancellationTokenParameterPosition { get; }

  public bool HasCancellationTokenParameter => CancellationTokenParameterPosition != -1;

  public abstract void Accept(IMethodDescriptorVisitor visitor);

  private IReadOnlyList<Type> GetParameterTypes()
  {
    return InterfaceMethod.GetParameters()
      .Select(p => p.ParameterType)
      .ToArray();
  }


  public static IEnumerable<MethodDescriptor> CreateMany(Type actorType, IImplementationMetadata implementationMetadata, Type interfaceType)
  {
    Type implementationType = implementationMetadata.ImplementationType;
    InterfaceMapping interfaceMapping = implementationType.GetInterfaceMap(interfaceType);

    for (int i = 0; i < interfaceMapping.TargetMethods.Length; i++)
    {
      MethodInfo interfaceMethod = interfaceMapping.InterfaceMethods[i];
      MethodInfo implementationMethod = interfaceMapping.TargetMethods[i];

      if (implementationMethod.IsGenericMethod)
        throw InvalidActorException.GenericMethodsAreNotSupported(implementationType, implementationMethod);

      IMethodMetadata methodMetadata = implementationMetadata.GetMethodMetadata(implementationMethod);

      yield return Create(actorType, interfaceMethod, implementationMethod, methodMetadata.IsStateless);
    }
  }

  private static MethodDescriptor Create(Type actorType, MethodInfo interfaceMethod, MethodInfo implementationMethod, bool isStateless)
  {
    Type implementationType = implementationMethod.DeclaringType;
    MethodKind methodKind = GetKind(actorType, implementationMethod);

    ParameterInfo[] parameters = implementationMethod.GetParameters();
    int cancellationTokenParameterPosition = -1;
    foreach (ParameterInfo parameter in parameters)
    {
      if (parameter.ParameterType == typeof(CancellationToken))
      {
        if (cancellationTokenParameterPosition != -1)
          throw InvalidActorException.DuplicateCancellationTokenParameters(implementationType, implementationMethod);

        cancellationTokenParameterPosition = parameter.Position;
      }
    }

    if (cancellationTokenParameterPosition != -1 && cancellationTokenParameterPosition != parameters.Length - 1)
      throw InvalidActorException.CancellationTokenMustBeLastParameter(implementationType, implementationMethod);

    return methodKind switch
    {
      MethodKind.Void       => new VoidMethodDescriptor(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition),
      MethodKind.Task       => new TaskMethodDescriptor(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition),
      MethodKind.TaskT      => new TaskTMethodDescriptor(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition),
      MethodKind.ValueTask  => new ValueTaskMethodDescriptor(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition),
      MethodKind.ValueTaskT => new ValueTaskTMethodDescriptor(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition),
      _                     => throw Errors.Unreacheable
    };
  }

  private static MethodKind GetKind(Type actorType, MethodInfo implementationMethod)
  {
    Type returnType = implementationMethod.ReturnType;
    if (returnType.IsGenericType)
    {
      returnType = returnType.GetGenericTypeDefinition();
    }

    if (returnType == typeof(void))
      return MethodKind.Void;

    if (returnType == typeof(Task))
      return MethodKind.Task;

    if (returnType == typeof(Task<>))
      return MethodKind.TaskT;

    if (returnType == typeof(ValueTask))
      return MethodKind.ValueTask;

    if (returnType == typeof(ValueTask<>))
      return MethodKind.ValueTaskT;

    throw InvalidActorException.InvalidMethodReturnType(actorType, implementationMethod);
  }
}
