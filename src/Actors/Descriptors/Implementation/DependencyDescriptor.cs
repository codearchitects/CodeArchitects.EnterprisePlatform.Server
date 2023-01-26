using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Common.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal abstract class DependencyDescriptor : IDependencyDescriptor
{
  protected DependencyDescriptor(ParameterInfo parameter)
  {
    Parameter = parameter;
  }

  public abstract DependencyKind Kind { get; }

  public abstract int CategoryIndex { get; }

  public ParameterInfo Parameter { get; }

  public string Name => Parameter.Name ?? $"$p{Index}";

  public Type Type => Parameter.ParameterType;

  public int Index => Parameter.Position;

  public abstract void Accept(IDependencyDescriptorVisitor visitor);


  public static IEnumerable<DependencyDescriptor> CreateMany(IActorMetadata actorMetadata, ConstructorInfo constructor)
  {
    CategoryIndices indices = new CategoryIndices();

    foreach (ParameterInfo parameter in constructor.GetParameters())
    {
      if (string.IsNullOrWhiteSpace(parameter.Name)) // This could only happen if the actor class is emitted dynamically
        throw new InvalidOperationException("Found a parameter with a null or whitespace name.");

      yield return Create(actorMetadata, parameter, indices);
    }
  }

  private static DependencyDescriptor Create(IActorMetadata actorMetadata, ParameterInfo parameter, CategoryIndices indices)
  {
    Type actorType = actorMetadata.ActorType;
    Type parameterType = parameter.ParameterType;

    if (TryGetStateField(actorMetadata, parameter, out FieldInfo? stateField))
      return new StateDependencyDescriptor(parameter, stateField, indices.StateIndex++);

    bool isNonGenericActorContext = parameterType == typeof(IActorContext);
    bool isGenericActorContext = parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(IActorContext<>);

    if (isNonGenericActorContext || isGenericActorContext)
    {
      if (isGenericActorContext && parameterType.GetGenericArguments()[0] != actorType)
        throw InvalidActorException.WrongGenericActorContext(actorType, parameter.Name);

      return new ContextDependencyDescriptor(parameter);
    }

    return new ServiceDependencyDescriptor(parameter, indices.ServicesIndex++)
    {
      IsOptional = parameter.HasDefaultValue && parameter.DefaultValue is null
    };
  }

  private static bool TryGetStateField(IActorMetadata actorMetadata, ParameterInfo parameter, [NotNullWhen(true)] out FieldInfo? stateField)
  {
    string parameterName = parameter.Name;

    foreach (IStateFieldMetadata metadata in actorMetadata.StateFields)
    {
      string fieldName = metadata.Field.Name;

      if (string.IsNullOrWhiteSpace(fieldName)) // This could only happen if the actor class is emitted dynamically
        throw new InvalidOperationException("Found a field with a null or whitespace name.");
      
      bool match =
        parameterName.MatchesUnderscorePrefixConvention(fieldName) ||
        parameterName.MatchesCamelCaseConvention(fieldName)        ||
        parameterName.MatchesAutoGenConvention(fieldName)          ||
        parameterName.MatchesMemberPrefixConvention(fieldName);

      if (match)
      {
        stateField = metadata.Field;
        return true;
      }
    }

    stateField = null;
    return false;
  }

  private class CategoryIndices
  {
    public int StateIndex { get; set; }
    public int ServicesIndex { get; set; }
  }
}
