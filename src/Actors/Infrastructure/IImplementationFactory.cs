namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IImplementationFactory<TActor, TState>
  where TActor : class
{
  TActor Create(IActorContextProvider<TActor> contextProvider, TState state);
}
