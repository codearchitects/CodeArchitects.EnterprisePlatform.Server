using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.Mapster;
using Mapster;
using Mapster.Models;
using MapsterMapper;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.MongoDB.Mapster;

/// <summary>
/// A <see cref="MongoDBMappedRepository{TDocument, TEntity, TKey}"/> that maps between the domain entity and
/// the document with Mapster.
/// </summary>
/// <typeparam name="TDocument">The document type stored in the collection.</typeparam>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
[Experimental]
public class MongoDBMapsterRepository<TDocument, TEntity, TKey> : MongoDBMappedRepository<TDocument, TEntity, TKey>
  where TDocument : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MongoDBMapsterRepository{TDocument, TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The MongoDB data context used by the repository.</param>
  /// <param name="mapper">The data layer mapper, whose configuration holds the rules for both directions.</param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="InvalidOperationException">The configuration has no rule for one of the two directions.</exception>
  public MongoDBMapsterRepository(IDataContext context, IDataMapper mapper)
    : base(context ?? throw new ArgumentNullException(nameof(context)))
  {
    Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    // The repository is created per scope: the configuration is checked once, not on every request.
    _ = s_validated.GetValue(mapper.Config, Validate);
  }

  /// <summary>
  /// The configurations already checked for this pair of types. Weak, so that a discarded configuration is not kept alive.
  /// </summary>
  private static readonly ConditionalWeakTable<TypeAdapterConfig, object> s_validated = new();

  /// <summary>
  /// The mapper used by the repository.
  /// </summary>
  protected IMapper Mapper { get; }

  private static object Validate(TypeAdapterConfig config)
  {
    EnsureRule<TEntity, TDocument>(config);
    EnsureRule<TDocument, TEntity>(config);

    return config;
  }

  /// <inheritdoc/>
  protected override TEntity TableToEntity(TDocument document) => Mapper.Map<TDocument, TEntity>(document);

  /// <inheritdoc/>
  protected override TDocument EntityToTable(TEntity entity) => Mapper.Map<TEntity, TDocument>(entity);

  private static void EnsureRule<TSource, TDestination>(TypeAdapterConfig config)
  {
    if (config.RuleMap.ContainsKey(new TypeTuple(typeof(TSource), typeof(TDestination))))
      return;

    if (!config.RequireExplicitMapping)
    {
      try
      {
        _ = config.GetMapFunction<TSource, TDestination>();
        return;
      }
      catch (Exception exception)
      {
        throw new InvalidOperationException(
          $"The implicit Mapster mapping from '{typeof(TSource).Name}' to '{typeof(TDestination).Name}' is invalid: " +
          $"{exception.Message} Register an explicit rule in AddDataMapster.", exception);
      }
    }

    throw new InvalidOperationException(
      $"No Mapster mapping from '{typeof(TSource).Name}' to '{typeof(TDestination).Name}' is configured, so the " +
      $"repository of '{typeof(TEntity).Name}' cannot map it. Register both directions in AddDataMapster, with " +
      $"config.NewConfig<{typeof(TEntity).Name}, {typeof(TDocument).Name}>().TwoWays() or with one rule per direction.");
  }
}
