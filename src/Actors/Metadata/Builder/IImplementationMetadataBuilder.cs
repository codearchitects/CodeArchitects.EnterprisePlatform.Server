using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

/// <summary>
/// A builder that can be used to configure an actor implementation.
/// </summary>
/// <typeparam name="TActor"></typeparam>
/// <typeparam name="TImplementation"></typeparam>
public interface IImplementationMetadataBuilder<TActor, TImplementation>
  where TActor : class
  where TImplementation : TActor
{
  /// <summary>
  /// Indicates whether the configured implementation is the default one.
  /// </summary>
  /// <param name="isDefault"><see langword="true"/> if the implementation is the default one, otherwise <see langword="false"/>.</param>
  /// <returns></returns>
  IImplementationMetadataBuilder<TActor, TImplementation> IsDefault(bool isDefault = true);

  /// <summary>
  /// Specifies which constructor to use for instantiating the actor.
  /// </summary>
  /// <param name="constructor">The actor's constructor.</param>
  /// <returns>The same builder.</returns>
  IImplementationMetadataBuilder<TActor, TImplementation> HasConstructor(ConstructorInfo constructor);

  /// <summary>
  /// Specifies the parameters of the constructor to use for instantiating the actor.
  /// </summary>
  /// <param name="parameterTypes">The actor's constructor parameters.</param>
  /// <returns>The same builder.</returns>
  IImplementationMetadataBuilder<TActor, TImplementation> HasConstructor(params Type[] parameterTypes);

  /// <summary>
  /// Specifies which constructor to use for instantiating the actor.
  /// </summary>
  /// <param name="constructorExpression">An expression that represents the creation of the actor.</param>
  /// <returns>The same builder.</returns>
  IImplementationMetadataBuilder<TActor, TImplementation> HasConstructor(Expression<Func<IConstructorArgument<TActor>, TImplementation>> constructorExpression);
}
