namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public interface IConstructorArgument<TActor>
  where TActor : class
{
  TArg OfType<TArg>();

  IActorContext<TActor> Context();
}
