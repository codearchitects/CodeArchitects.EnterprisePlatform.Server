using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public interface IStateFieldMetadataBuilder<TState>
{
  IStateFieldMetadataBuilder<TState> HasDefaultValue(TState defaultValue);
  
  IStateFieldMetadataBuilder<TState> IsActorId();
  
  IStateFieldMetadataBuilder<TState> IsActorIdSource<TProperty>(Expression<Func<TState, TProperty>> idPropertyExpression);
}
