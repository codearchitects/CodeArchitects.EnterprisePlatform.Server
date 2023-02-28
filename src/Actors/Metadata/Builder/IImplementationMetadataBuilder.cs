using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public interface IImplementationMetadataBuilder<TActor, TImplementation>
  where TActor : class
  where TImplementation : TActor
{
  IImplementationMetadataBuilder<TActor, TImplementation> IsDefault(bool isDefault = true);

  IImplementationMetadataBuilder<TActor, TImplementation> HasConstructor(ConstructorInfo constructor);

  IImplementationMetadataBuilder<TActor, TImplementation> HasConstructor(params Type[] parameterTypes);

  IImplementationMetadataBuilder<TActor, TImplementation> HasConstructor(Expression<Func<IConstructorArgument<TActor>, TImplementation>> constructorExpression);
}
