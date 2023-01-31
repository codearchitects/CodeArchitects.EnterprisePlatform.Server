namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorContextProvider<TActor>
  where TActor : class
{
  IActorContext<TActor, TImplementation> GetContext<TImplementation>()
    where TImplementation : class, TActor;
}
