namespace CodeArchitects.Platform.Actors.Metadata.Builder;

/// <summary>
/// Specifies the type of the arguments of an actor constructor.
/// </summary>
/// <typeparam name="TActor"></typeparam>
public interface IConstructorArgument<TActor>
  where TActor : class
{
  /// <summary>
  /// Indicates a constructor arguments is of a specified type.
  /// </summary>
  /// <typeparam name="TArg">The type of the argument.</typeparam>
  TArg OfType<TArg>();

  /// <summary>
  /// Indicates a constructor arguments is an actor context.
  /// </summary>
  /// <returns></returns>
  IActorContext<TActor> Context();
}
