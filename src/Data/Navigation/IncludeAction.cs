namespace CodeArchitects.Platform.Data.Navigation;

/// <summary>
/// Specifies which related entities to include in the query.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <param name="includer">The includer object that can be used to specify the entities to include.</param>
public delegate void IncludeAction<TEntity>(IIncluder<TEntity> includer)
  where TEntity : class;