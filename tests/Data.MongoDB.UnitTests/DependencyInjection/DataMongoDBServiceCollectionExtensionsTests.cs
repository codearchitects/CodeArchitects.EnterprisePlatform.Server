using CodeArchitects.Platform.Data.MongoDB.Collections;
using CodeArchitects.Platform.Data.MongoDB.Filters;
using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model;
using CodeArchitects.Platform.Data.MongoDB.Transactions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.DependencyInjection;

public class DataMongoDBServiceCollectionExtensionsTests
{
  private readonly Mock<IMongoClient> _client = new();

  public DataMongoDBServiceCollectionExtensionsTests()
  {
    _client
      .Setup(client => client.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
      .Returns(Mock.Of<IMongoDatabase>());
  }

  private ServiceProvider BuildProvider(
    Action<IMongoDBConfigurationBuilderWithDatabase>? configure = null)
  {
    IServiceCollection services = new ServiceCollection().AddData(options =>
    {
      IMongoDBConfigurationBuilderWithDatabase builder = options
        .UseClient(_client.Object)
        .UseDatabase("tests")
        .AddEntity<DiscoverableEntity>();

      configure?.Invoke(builder);
      return builder;
    });

    // ValidateScopes proves there is no captive dependency: a singleton capturing a scoped
    // service would throw here rather than misbehave at runtime.
    return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
  }

  #region Validation

  [Fact]
  public void AddData_ShouldThrow_WhenTheClientIsNotConfigured()
  {
    // Arrange
    MongoDBConfigurationBuilder builder = new();
    builder.UseDatabase("tests").AddEntity<DiscoverableEntity>();

    // Act
    Action act = () => builder.AddServices(new ServiceCollection());

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*UseConnectionString*");
  }

  [Fact]
  public void AddData_ShouldThrow_WhenTheDatabaseIsNotConfigured()
  {
    // Arrange
    MongoDBConfigurationBuilder builder = new();
    builder.UseClient(_client.Object);

    // Act
    Action act = () => builder.AddServices(new ServiceCollection());

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*UseDatabase*");
  }

  [Fact]
  public void AddData_ShouldThrow_WhenNoEntityIsRegistered()
  {
    // Act
    Action act = () => new ServiceCollection().AddData(options => options
      .UseClient(_client.Object)
      .UseDatabase("tests"));

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*AddEntitiesFrom*");
  }

  [Fact]
  public void AddData_ShouldThrow_WhenTwoEntitiesShareTheSameCollection()
  {
    // Act
    Action act = () => new ServiceCollection().AddData(options => options
      .UseClient(_client.Object)
      .UseDatabase("tests")
      .AddEntity<DiscoverableEntity>()
      .AddEntity<CollidingEntity>());

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*discoveredEntities*");
  }

  [Fact]
  public void AddData_ShouldDiscoverPublicAttributedEntities()
  {
    // Act
    using ServiceProvider provider = new ServiceCollection()
      .AddData(options => options
        .UseClient(_client.Object)
        .UseDatabase("tests")
        .AddEntitiesFrom(typeof(DiscoverableEntity).Assembly))
      .BuildServiceProvider();

    // Assert
    provider.GetRequiredService<IDataModel>()
      .TryGetEntity(typeof(DiscoverableEntity), out _)
      .Should().BeTrue();
  }

  #endregion

  #region Registrations

  [Fact]
  public void AddData_ShouldRegisterTheProviderAgnosticContract()
  {
    // Arrange
    using ServiceProvider provider = BuildProvider();
    using IServiceScope scope = provider.CreateScope();

    // Act
    Data.IDataContext baseContract = scope.ServiceProvider.GetRequiredService<Data.IDataContext>();
    IDataContext mongoContract = scope.ServiceProvider.GetRequiredService<IDataContext>();

    // Assert
    // Generated, provider-agnostic code depends on the base contract: both must resolve,
    // and to the same instance.
    baseContract.Should().BeSameAs(mongoContract);
  }

  [Theory]
  [InlineData(typeof(IDataContext))]
  [InlineData(typeof(IUnitOfWorkManager))]
  [InlineData(typeof(IUnitOfWork))]
  [InlineData(typeof(ISeeder))]
  public void AddData_ShouldRegisterTheScopedServices(Type serviceType)
  {
    // Arrange
    using ServiceProvider provider = BuildProvider();
    using IServiceScope scope = provider.CreateScope();

    // Act
    object? service = scope.ServiceProvider.GetService(serviceType);

    // Assert
    service.Should().NotBeNull();
  }

  [Theory]
  [InlineData(typeof(IMongoClient))]
  [InlineData(typeof(IMongoDatabase))]
  [InlineData(typeof(IDataModel))]
  [InlineData(typeof(IFilterProvider))]
  [InlineData(typeof(ICollectionProvider))]
  [InlineData(typeof(ITransactionCapabilityProbe))]
  public void AddData_ShouldRegisterTheSingletonServices(Type serviceType)
  {
    // Arrange
    using ServiceProvider provider = BuildProvider();

    // Act
    object? first = provider.GetService(serviceType);
    object? second = provider.GetService(serviceType);

    // Assert
    first.Should().NotBeNull().And.BeSameAs(second);
  }

  [Fact]
  public void AddData_ShouldScopeTheDataContextPerScope()
  {
    // Arrange
    using ServiceProvider provider = BuildProvider();

    // Act
    using IServiceScope first = provider.CreateScope();
    using IServiceScope second = provider.CreateScope();

    // Assert
    first.ServiceProvider.GetRequiredService<IDataContext>()
      .Should().NotBeSameAs(second.ServiceProvider.GetRequiredService<IDataContext>());
  }

  [Fact]
  public void AddData_ShouldApplyTheConfiguredTransactionMode()
  {
    // Arrange
    using ServiceProvider provider = BuildProvider(
      builder => builder.UseTransactions(TransactionMode.Disabled));

    // Act
    MongoDBOptions options = provider.GetRequiredService<MongoDBOptions>();

    // Assert
    options.TransactionMode.Should().Be(TransactionMode.Disabled);
  }

  [Fact]
  public void AddData_ShouldRegisterTheSeed_WhenConfigured()
  {
    // Arrange
    using ServiceProvider provider = BuildProvider(builder => builder.UseSeed<TestSeed>());
    using IServiceScope scope = provider.CreateScope();

    // Act
    DataSeed seed = scope.ServiceProvider.GetRequiredService<DataSeed>();

    // Assert
    seed.Should().BeOfType<TestSeed>();
  }

  [Fact]
  public void AddData_ShouldRegisterEverySeed_WhenSeveralAreConfigured()
  {
    // Arrange
    using ServiceProvider provider = BuildProvider(builder => builder
      .UseSeed<TestSeed>()
      .UseSeed<OtherTestSeed>());
    using IServiceScope scope = provider.CreateScope();

    // Act
    DataSeed[] seeds = [.. scope.ServiceProvider.GetServices<DataSeed>()];

    // Assert
    seeds.Should().HaveCount(2);
  }

  [Fact]
  public void UseSeed_ShouldIgnoreDuplicates()
  {
    // Arrange
    using ServiceProvider provider = BuildProvider(builder => builder
      .UseSeed<TestSeed>()
      .UseSeed<TestSeed>());
    using IServiceScope scope = provider.CreateScope();

    // Act
    DataSeed[] seeds = [.. scope.ServiceProvider.GetServices<DataSeed>()];

    // Assert
    // Registering the same seed twice would apply it twice within one commit.
    seeds.Should().ContainSingle();
  }

  [Fact]
  public void AddSeedsFrom_ShouldDiscoverConcreteSeeds()
  {
    // Arrange
    MongoDBConfigurationBuilder builder = new();
    builder.UseClient(_client.Object).UseDatabase("tests").AddEntity<DiscoverableEntity>();

    // Act
    builder.AddSeedsFrom(typeof(DiscoverableSeed).Assembly);

    ServiceCollection services = new();
    builder.AddServices(services);
    using ServiceProvider provider = services.BuildServiceProvider();
    using IServiceScope scope = provider.CreateScope();

    // Assert
    scope.ServiceProvider.GetServices<DataSeed>()
      .Should().ContainSingle(seed => seed is DiscoverableSeed);
  }

  [Fact]
  public void SeedMongo_ShouldNotThrow_WhenNoSeedIsRegistered()
  {
    // Arrange
    using ServiceProvider provider = BuildProvider();

    // Act
    Action act = provider.SeedMongo;

    // Assert
    // Nothing to do is not an error: it is logged and skipped.
    act.Should().NotThrow();
  }

  [Fact]
  public void UseSeed_ShouldThrow_WhenTheTypeIsNotADataSeed()
  {
    // Arrange
    MongoDBConfigurationBuilder builder = new();

    // Act
    Action act = () => builder.UseSeed(typeof(DiscoverableEntity));

    // Assert
    act.Should().Throw<ArgumentException>();
  }

  #endregion

  private sealed class TestSeed : DataSeed
  {
    public override void Seed(ISeeder seeder)
    {
    }
  }

  private sealed class OtherTestSeed : DataSeed
  {
    public override void Seed(ISeeder seeder)
    {
    }
  }
}
