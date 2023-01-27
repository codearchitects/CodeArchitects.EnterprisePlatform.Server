using CodeArchitects.Platform.Actors.Metadata.Implementation;
using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.Common.Utils;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

internal class ImplementationMetadataBuilder<TImplementation> : ImplementationMetadata, IImplementationMetadataBuilder<TImplementation>
  where TImplementation : class
{
  private readonly Type _actorType;
  private readonly Action<StateFieldMetadata> _processMetadata;

  public ImplementationMetadataBuilder(Type actorType, Action<StateFieldMetadata> processMetadata)
  {
    _actorType = actorType;
    _processMetadata = processMetadata;
  }

  private bool _isDefault;
  private bool _hasStateFields;
  private ConstructorInfo? _constructor;

  public override bool IsDefault => _isDefault;

  public override Type ImplementationType => typeof(TImplementation);

  public override ConstructorInfo? Constructor => _constructor;

  public override bool HasStateFields => _hasStateFields;

  public IImplementationMetadataBuilder<TImplementation> AsBuilder() => this;

  IImplementationMetadataBuilder<TImplementation> IImplementationMetadataBuilder<TImplementation>.HasActorConstructor(Expression<Func<IConstructorArgumentSpec, TImplementation>> constructorExpression)
  {
    if (constructorExpression is null)
      throw new ArgumentNullException(nameof(constructorExpression));

    if (constructorExpression.Body is not NewExpression newExpression)
      throw new ArgumentException("The expression should invoke an actor constructor.");

    ConstructorInfo constructor = newExpression.Constructor!;
    ParameterInfo[] parameters = constructor.GetParameters();
    ParameterExpression argParameter = constructorExpression.Parameters[0];

    for (int i = 0; i < parameters.Length; i++)
    {
      ProcessConstructorArgument(parameters[i], argParameter, newExpression.Arguments[i]);
    }

    _constructor = constructor;

    return this;
  }

  IImplementationMetadataBuilder<TImplementation> IImplementationMetadataBuilder<TImplementation>.IsDefault(bool isDefault)
  {
    _isDefault = IsDefault;

    return this;
  }

  IImplementationMetadataBuilder<TImplementation> IImplementationMetadataBuilder<TImplementation>.HasMethod<TReturn>(Expression<Func<TImplementation, IMethodArgumentSpec, TReturn>> methodExpression, Action<IMethodMetadataBuilder> configure)
  {
    if (methodExpression is null)
      throw new ArgumentNullException(nameof(methodExpression));

    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    if (methodExpression.Body is not MethodCallExpression methodCallExpression || methodCallExpression.Object is null || methodCallExpression.Object.Type != ImplementationType)
      throw new ArgumentException("The expression should call an actor method.");

    MethodInfo implementationMethod = methodCallExpression.Method;
    MethodMetadataBuilder builder = new(implementationMethod);
    configure(builder);
    AddMethodMetadata(builder);

    return this;
  }

  private FieldInfo GetStateField(string parameterName)
  {
    if (TryGetStateField(_actorType, parameterName, out FieldInfo? stateField))
      return stateField;

    if (TryGetStateField(ImplementationType, parameterName, out stateField))
      return stateField;

    throw InvalidActorException.StateComponentsMismatch(_actorType);
  }

  private bool TryGetStateField(Type type, string parameterName, [NotNullWhen(true)] out FieldInfo? stateField)
  {
    // TODO: We need a better metadata abstraction due to the fact we determine state components in different ways when using Builder and Reflection. This also results in duplicate code when looking for the state field.
    stateField = null;
    
    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    {
      string fieldName = field.Name;

      bool match =
        parameterName.MatchesUnderscorePrefixConvention(fieldName) ||
        parameterName.MatchesCamelCaseConvention(fieldName)        ||
        parameterName.MatchesAutoGenConvention(fieldName)          ||
        parameterName.MatchesMemberPrefixConvention(fieldName);

      if (match)
      {
        if (stateField is not null)
          throw InvalidActorException.StateComponentsMismatch(_actorType);

        stateField = field;
      }
    }

    return stateField is not null;
  }

  private void ProcessConstructorArgument(ParameterInfo parameter, ParameterExpression argParameter, Expression argumentExpression)
  {
    if (argumentExpression is not MethodCallExpression methodCallExpression)
      return;
    if (methodCallExpression.Object != argParameter)
      return;
    if (methodCallExpression.Method.Name != nameof(IConstructorArgumentSpec.State))
      return;

    FieldInfo field = GetStateField(parameter.Name);
    StateFieldMetadata metadata = CreateStateField(field, methodCallExpression);
    _processMetadata(metadata);
    _hasStateFields = true;
  }

  private static StateFieldMetadata CreateStateField(FieldInfo field, MethodCallExpression methodCallExpression)
  {
    MethodInfo createStateDependencyMethod = typeof(ImplementationMetadataBuilder<TImplementation>).GetRequiredMethod(
      name: nameof(CreateStateField),
      bindingAttr: BindingFlags.Static | BindingFlags.NonPublic,
      types: new[] { typeof(FieldInfo), typeof(IReadOnlyList<Expression>) });

    Type stateType = methodCallExpression.Method.GetGenericArguments()[0];
    return (StateFieldMetadata)createStateDependencyMethod
      .MakeGenericMethod(stateType)
      .Invoke(null, new object?[] { field, methodCallExpression.Arguments });
  }

  private static StateFieldMetadata CreateStateField<TState>(FieldInfo field, IReadOnlyList<Expression> arguments)
  {
    StateFieldMetadataBuilder<TState> builder = new(field);
    if (arguments.Count == 0)
      return builder;

    Expression argument = arguments[0];

    Debug.Assert(argument.Type == typeof(Action<IStateFieldMetadataBuilder<TState>>));

    Action<IStateFieldMetadataBuilder<TState>> configure = ExpressionEvaluator.Evaluate<Action<IStateFieldMetadataBuilder<TState>>>(argument);
    configure(builder);

    return builder;
  }
}
