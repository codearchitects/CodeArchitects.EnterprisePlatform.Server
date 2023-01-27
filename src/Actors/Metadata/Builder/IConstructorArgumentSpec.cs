namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public interface IConstructorArgumentSpec : IMethodArgumentSpec
{
  TArg State<TArg>();
  
  TArg State<TArg>(Action<IStateFieldMetadataBuilder<TArg>> configure);
  
  TArg? Optional<TArg>()
    where TArg : class;
}
