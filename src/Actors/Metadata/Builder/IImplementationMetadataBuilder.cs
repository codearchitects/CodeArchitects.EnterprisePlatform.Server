using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public interface IImplementationMetadataBuilder<TImplementation>
  where TImplementation : class
{
  IImplementationMetadataBuilder<TImplementation> HasActorConstructor(Expression<Func<IConstructorArgumentSpec, TImplementation>> constructorExpression);

  IImplementationMetadataBuilder<TImplementation> HasMethod<TReturn>(Expression<Func<TImplementation, IMethodArgumentSpec, TReturn>> methodExpression, Action<IMethodMetadataBuilder> configure);

  IImplementationMetadataBuilder<TImplementation> IsDefault(bool isDefault = true);
}
