using CodeArchitects.Platform.Common.CodeAnalysis;
using MongoDB.Driver;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.DependencyInjection;

/// <summary>
/// An object used to configure the MongoDB context and services.
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
  /// Specify the client used for the database communication.
  /// </summary>
  /// <param name="client">The <see cref="MongoClient"/> client.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilderWithClient"/> for further MongoDB client configuration.</returns>
  IMongoDBConfigurationBuilderWithClient UseClient(MongoClient client);

  /// <summary>
  /// Specifies the seed type to use for seeding the database.
  /// </summary>
  /// <param name="seedType">The seed type. It must extend <see cref="DataSeed"/>.</param>
  /// <returns>An <see cref="IMongoDBConfigurationBuilder"/> for further configurations.</returns>
  IMongoDBConfigurationBuilder UseSeed(Type seedType);
}

/// <summary>
/// An object used to configure the MongoDB client.
/// </summary>
public interface IMongoDBConfigurationBuilderWithClient
{
  /// <summary>
  /// Specifies the database to use.
  /// </summary>
  /// <param name="databaseName">The database name.</param>
  /// <param name="settings">The database settings.</param>
  /// <returns>And <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further MongoDB database configuration.</returns>
  IMongoDBConfigurationBuilderWithDatabase UseDatabase(string databaseName, MongoDatabaseSettings? settings = null);
}

/// <summary>
/// An object used to configure the MongoDB database.
/// </summary>
public interface IMongoDBConfigurationBuilderWithDatabase
{
  /// <summary>
  /// Specifies the assembly to retrieve the entities from.
  /// </summary>
  /// <param name="assembly">The source assembly.</param>
  /// <returns>And <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further MongoDB database configuration.</returns>
  IMongoDBConfigurationBuilderWithDatabase AddEntitiesFrom(Assembly assembly);
}
