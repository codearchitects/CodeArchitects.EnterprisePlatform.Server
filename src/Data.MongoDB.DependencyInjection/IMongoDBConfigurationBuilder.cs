using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// An object used to configure the MongoDB client.
/// </summary>
[Experimental]
public interface IMongoDBConfigurationBuilder
{
  /// <summary>
  /// Specify the connection string used for the database connection.
  /// </summary>
  /// <param name="connectionString">The MongoDB connection string.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithClient"/> for further MongoDB client configuration.</returns>
  IMongoDBConfigurationBuilderWithClient UseConnectionString(string connectionString);

  /// <summary>
  /// Specify the connection string used for the database connection, and adjust the resulting
  /// client settings.
  /// </summary>
  /// <param name="connectionString">The MongoDB connection string.</param>
  /// <param name="configure">Applied to the settings parsed from the connection string.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithClient"/> for further MongoDB client configuration.</returns>
  IMongoDBConfigurationBuilderWithClient UseConnectionString(string connectionString, Action<MongoClientSettings> configure);

  /// <summary>
  /// Specify the client used for the database communication.
  /// </summary>
  /// <param name="client">The MongoDB client.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithClient"/> for further MongoDB client configuration.</returns>
  IMongoDBConfigurationBuilderWithClient UseClient(IMongoClient client);

  /// <summary>
  /// Specify how to build the client used for the database communication.
  /// </summary>
  /// <param name="factory">Resolves the client from the application services.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithClient"/> for further MongoDB client configuration.</returns>
  IMongoDBConfigurationBuilderWithClient UseClient(Func<IServiceProvider, IMongoClient> factory);
}

/// <summary>
/// An object used to configure the MongoDB database.
/// </summary>
[Experimental]
public interface IMongoDBConfigurationBuilderWithClient
{
  /// <summary>
  /// Specifies the database to use.
  /// </summary>
  /// <param name="databaseName">The database name.</param>
  /// <param name="settings">The database settings.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further MongoDB database configuration.</returns>
  IMongoDBConfigurationBuilderWithDatabase UseDatabase(string databaseName, MongoDatabaseSettings? settings = null);
}

/// <summary>
/// An object used to configure the MongoDB model and behaviour.
/// </summary>
[Experimental]
public interface IMongoDBConfigurationBuilderWithDatabase
{
  /// <summary>
  /// Registers every entity of an assembly, that is every public, concrete, non-generic class
  /// marked with <c>[Collection]</c> or <c>[Table]</c>.
  /// </summary>
  /// <param name="assembly">The source assembly.</param>
  /// <param name="filter">An optional additional filter over the candidate types.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further MongoDB database configuration.</returns>
  IMongoDBConfigurationBuilderWithDatabase AddEntitiesFrom(Assembly assembly, Func<Type, bool>? filter = null);

  /// <summary>
  /// Registers a single entity type, whether or not it carries a discovery attribute.
  /// </summary>
  /// <param name="entityType">The entity type.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further MongoDB database configuration.</returns>
  IMongoDBConfigurationBuilderWithDatabase AddEntity(Type entityType);

  /// <summary>
  /// Specifies how the provider behaves when an operation needs a MongoDB transaction.
  /// </summary>
  /// <param name="mode">The transaction mode. Defaults to <see cref="TransactionMode.Required"/>.</param>
  /// <param name="options">Read/write concern and timeout applied to the transactions.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further MongoDB database configuration.</returns>
  IMongoDBConfigurationBuilderWithDatabase UseTransactions(TransactionMode mode, TransactionOptions? options = null);

  /// <summary>
  /// Specifies how <see cref="Guid"/> values are stored. Defaults to
  /// <see cref="GuidRepresentation.Standard"/>.
  /// </summary>
  /// <param name="representation">The representation to use.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further MongoDB database configuration.</returns>
  IMongoDBConfigurationBuilderWithDatabase UseGuidRepresentation(GuidRepresentation representation);

  /// <summary>
  /// Adds conventions to the pack the provider registers for its entities.
  /// </summary>
  /// <param name="configure">Applied to the provider convention pack.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further MongoDB database configuration.</returns>
  IMongoDBConfigurationBuilderWithDatabase ConfigureConventions(Action<ConventionPack> configure);

  /// <summary>
  /// Specifies the seed type to use for seeding the database.
  /// </summary>
  /// <param name="seedType">The seed type. It must extend <see cref="DataSeed"/>.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further MongoDB database configuration.</returns>
  IMongoDBConfigurationBuilderWithDatabase UseSeed(Type seedType);
}
