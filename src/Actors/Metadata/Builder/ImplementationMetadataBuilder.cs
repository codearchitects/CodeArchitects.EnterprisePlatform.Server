using CodeArchitects.Platform.Actors.Metadata.Factory;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

internal class ImplementationMetadataBuilder<TActor, TImplementation> : ImplementationDescriptorFactory<TActor>, IImplementationMetadataBuilder<TActor, TImplementation>
  where TActor : class
  where TImplementation : TActor
{
  private bool _isDefault;
  private ConstructorInfo? _constructor;

  public ImplementationMetadataBuilder(int id, ActorMetadataBuilder<TActor> builder)
    : base(id, builder)
  {
  }

  public override bool IsDefault => _isDefault;

  public override Type ImplementationType => typeof(TImplementation);

  protected override bool DefinesStateMembers => false;

  protected override ConstructorInfo? Constructor => _constructor;

  public IImplementationMetadataBuilder<TActor, TImplementation> AsBuilder() => this;

  IImplementationMetadataBuilder<TActor, TImplementation> IImplementationMetadataBuilder<TActor, TImplementation>.HasConstructor(ConstructorInfo constructor)
  {
    if (constructor is null)
      throw new ArgumentNullException(nameof(constructor));

    _constructor = constructor;
    return this;
  }

  IImplementationMetadataBuilder<TActor, TImplementation> IImplementationMetadataBuilder<TActor, TImplementation>.HasConstructor(params Type[] parameterTypes)
  {
    if (parameterTypes is null)
      throw new ArgumentNullException(nameof(parameterTypes));

    _constructor = ImplementationType.GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
      types: parameterTypes);
    return this;
  }

  IImplementationMetadataBuilder<TActor, TImplementation> IImplementationMetadataBuilder<TActor, TImplementation>.HasConstructor(Expression<Func<IConstructorArgument<TActor>, TImplementation>> constructorExpression)
  {
    if (constructorExpression is null)
      throw new ArgumentNullException(nameof(constructorExpression));

    if (constructorExpression.Body is not NewExpression newExpression)
      throw new ArgumentException("The constructor expression must call the actor's constructor.", nameof(constructorExpression));

    _constructor = newExpression.Constructor;
    return this;
  }

  IImplementationMetadataBuilder<TActor, TImplementation> IImplementationMetadataBuilder<TActor, TImplementation>.IsDefault(bool isDefault)
  {
    _isDefault = isDefault;
    return this;
  }
}
