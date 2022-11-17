namespace CodeArchitects.Platform.Data.Navigation;

public interface IIncluder<TEntity> : IExpressionIncluder<TEntity>, IStringIncluder<TEntity>
  where TEntity : class
{
}
