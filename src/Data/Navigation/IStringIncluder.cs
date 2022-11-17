namespace CodeArchitects.Platform.Data.Navigation;

public interface IStringIncluder<TEntity>
  where TEntity : class
{
  IStringIncluder<TEntity> Include(string navigation);
}
