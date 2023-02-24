using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal abstract class MethodDescriptor : IMethodDescriptor
{
  protected MethodDescriptor(int id, MethodInfo? interfaceMethod, MethodInfo implementationMethod, Type[] parameterTypes, Func<IMethodDescriptor, Type> activityTypeFactory)
  {
    Id = id;
    InterfaceMethod = interfaceMethod;
    ImplementationMethod = implementationMethod;
    ParameterTypes = parameterTypes;
    ActivityType = activityTypeFactory(this);
  }

  public abstract MethodKind Kind { get; }

  public int Id { get; }

  public string Name => InterfaceMethod?.Name ?? ImplementationMethod.Name;

  public MethodInfo? InterfaceMethod { get; }

  public MethodInfo ImplementationMethod { get; }

  public Type ReturnType => ImplementationMethod.ReturnType;

  public Type[] ParameterTypes { get; }

  public Type ActivityType { get; }

  public IReadOnlyList<FieldInfo> ActivityFields => ActivityType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

  public bool HasCancellationTokenParameter => ParameterTypes.Length != 0 && ParameterTypes[ParameterTypes.Length - 1] == typeof(CancellationToken);

  public abstract void Accept(IMethodDescriptorVisitor visitor);
}
