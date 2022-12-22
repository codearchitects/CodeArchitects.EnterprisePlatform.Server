using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Materialization;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Oracle.Command;
using CodeArchitects.Platform.Data.AdoNet.PostgreSQL.Command;
using CodeArchitects.Platform.Data.AdoNet.SQLServer.Command;
using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Query;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.Data.SqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace CodeArchitects.Platform.Data.Fixtures;

internal class RepositoryFactory
{
  private readonly TestFixture _fixture;

  public RepositoryFactory(TestFixture fixture)
  {
    _fixture = fixture;
  }

  public Repository<TEntity, TKey> CreateRepository<TEntity, TKey>(RepositoryDependencies dependencies)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    var (implementation, provider, trackingContext) = dependencies;

    switch (implementation)
    {
      case RepositoryImplementation.AdoNet:
        return CreateAdoNetRepository<TEntity, TKey>(provider, trackingContext);
      case RepositoryImplementation.EFCore:
        return CreateEFCoreRepository<TEntity, TKey>(trackingContext);
      default:
        throw Errors.Unreacheable;
    }
  }

  private AdoNetRepository<TEntity, TKey> CreateAdoNetRepository<TEntity, TKey>(DbProvider provider, ITrackingContext trackingContext)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    SqlTextCache sqlCache = SqlTextCache.Create();
    IdentityCollectionFactory collectionFactory = IdentityCollectionFactory.Create();
    IDataModel dataModel = new TestModelConfiguration().CreateDataModel();

    AdoNet.IDataContext dataContext = provider switch
    {
      DbProvider.SqlServer => CreateSqlServerContext(),
      DbProvider.Postgres  => CreatePostgresContext(),
      DbProvider.Oracle    => CreateOracleContext(),
      _                    => throw Errors.Unreacheable
    };

    return new AdoNetRepository<TEntity, TKey>(dataContext);

    AdoNet.IDataContext CreateSqlServerContext()
    {
      DelegateConnectionFactory<SqlConnection> connectionFactory = new(() => _fixture.SqlServerConnection);
      AdoNet.StateManager<SqlConnection> stateManager = new(connectionFactory);
      SqlTextBuilder sqlBuilder = new(sqlCache, new SQLServerSyntaxProvider());
      CommandBuilder<SqlCommand> commandBuilder = new(sqlBuilder);
      RowReaderProvider rowReaderProvider = RowReaderProvider.Create();
      Materializer materializer = new(collectionFactory, rowReaderProvider);
      ICommandInterceptorAggregator<SqlCommand> interceptor = Mock.Of<ICommandInterceptorAggregator<SqlCommand>>();
      Executor<SqlCommand> executor = new(commandBuilder, materializer, interceptor, trackingContext);

      return new DataContext<SqlConnection, SqlCommand>(stateManager, executor, interceptor, dataModel);
    }

    AdoNet.IDataContext CreatePostgresContext()
    {
      DelegateConnectionFactory<NpgsqlConnection> connectionFactory = new(() => _fixture.PostgresConnection);
      AdoNet.StateManager<NpgsqlConnection> stateManager = new(connectionFactory);
      SqlTextBuilder sqlBuilder = new(sqlCache, new PostgreSQLSyntaxProvider());
      CommandBuilder<NpgsqlCommand> commandBuilder = new(sqlBuilder);
      RowReaderProvider rowReaderProvider = RowReaderProvider.Create();
      Materializer materializer = new(collectionFactory, rowReaderProvider);
      ICommandInterceptorAggregator<NpgsqlCommand> interceptor = Mock.Of<ICommandInterceptorAggregator<NpgsqlCommand>>();
      Executor<NpgsqlCommand> executor = new(commandBuilder, materializer, interceptor, trackingContext);

      return new DataContext<NpgsqlConnection, NpgsqlCommand>(stateManager, executor, interceptor, dataModel);
    }

    AdoNet.IDataContext CreateOracleContext()
    {
      DelegateConnectionFactory<OracleConnection> connectionFactory = new(() => _fixture.OracleConnection);
      AdoNet.StateManager<OracleConnection> stateManager = new(connectionFactory);
      SqlTextBuilder sqlBuilder = new(sqlCache, new OracleSyntaxProvider());
      AdoNet.Oracle.Command.OracleCommandBuilder commandBuilder = new(sqlBuilder);
      RowReaderProvider rowReaderProvider = RowReaderProvider.Create();
      Materializer materializer = new(collectionFactory, rowReaderProvider);
      ICommandInterceptorAggregator<OracleCommand> interceptor = Mock.Of<ICommandInterceptorAggregator<OracleCommand>>();
      Executor<OracleCommand> executor = new(commandBuilder, materializer, interceptor, trackingContext);

      return new DataContext<OracleConnection, OracleCommand>(stateManager, executor, interceptor, dataModel);
    }
  }

  private EFCoreRepository<TEntity, TKey> CreateEFCoreRepository<TEntity, TKey>(ITrackingContext trackingContext)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    EntityFrameworkCore.StateManager<TestDbContext> stateManager = new(_fixture.DbContext);

    PredicateTemplateFactory templateFactory = new(_fixture.DbContext.Model);
    PredicateProvider predicateProvider = new(templateFactory, PredicateTemplateCache.Create());

    DefaultEntityFactoryFactory entityFactoryFactory = new(_fixture.DbContext.Model);
    DefaultEntityFactory entityFactory = new(entityFactoryFactory, DefaultEntityFactoryCache.Create());

    DataContext<TestDbContext> dataContext = new(stateManager, trackingContext, predicateProvider, entityFactory);

    return new EFCoreRepository<TEntity, TKey>(dataContext);
  }
}
