using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

public interface IStateComponentBuilder<TActor, TState>
  where TActor : class
{
  IStateComponentBuilder<TActor, TState> HasDefaultValue(TState value);

  IStateComponentBuilder<TActor, TState> IsActorId(bool isActorId = true);

  IStateComponentBuilder<TActor, TState> IsActorIdSource<TProperty>(Expression<Func<TState, TProperty>> memberExpression);

  IStateComponentBuilder<TActor, TState> IsActorIdSource(string memberName);
}
