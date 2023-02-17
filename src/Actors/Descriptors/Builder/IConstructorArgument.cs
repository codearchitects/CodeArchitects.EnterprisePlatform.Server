namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

public interface IConstructorArgument<TActor>
  where TActor : class
{
  TArg OfType<TArg>();

  IActorContext<TActor> Context();
}
