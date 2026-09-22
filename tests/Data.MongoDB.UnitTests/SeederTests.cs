using CodeArchitects.Platform.Data.MongoDB.Collections;
using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

public class SeederTests
{
  private readonly Mock<ICollectionProvider> _collections = new();
  private readonly Mock<IStateManager> _stateManager = new();

  public SeederTests()
  {
    _collections
      .Setup(collections => collections.GetCollection<DiscoverableEntity>())
      .Returns(Mock.Of<IMongoCollection<DiscoverableEntity>>());
  }

  private Seeder CreateSut() => new(_collections.Object, _stateManager.Object);

  #region Ordering

  [Fact]
  public void Order_ShouldHonourTheDeclaredOrder()
  {
    // Arrange
    DataSeed[] seeds = [new ThirdSeed(), new FirstSeed(), new SecondSeed()];

    // Act
    DataSeed[] ordered = [.. Seeder.Order(seeds)];

    // Assert
    ordered.Select(seed => seed.GetType())
      .Should().Equal(typeof(FirstSeed), typeof(SecondSeed), typeof(ThirdSeed));
  }

  [Fact]
  public void Order_ShouldBeDeterministic_WhenNoOrderIsDeclared()
  {
    // Arrange
    DataSeed[] seeds = [new UnorderedB(), new UnorderedA()];

    // Act
    DataSeed[] first = [.. Seeder.Order(seeds)];
    // AsEnumerable: on an array, Reverse() would bind to MemoryExtensions.Reverse(Span<T>),
    // which reverses in place and returns void.
    DataSeed[] second = [.. Seeder.Order(seeds.AsEnumerable().Reverse())];

    // Assert
    // Neither assembly scanning nor service resolution guarantees a stable order:
    // the type name is the tie-breaker.
    first.Select(seed => seed.GetType()).Should().Equal(typeof(UnorderedA), typeof(UnorderedB));
    second.Select(seed => seed.GetType()).Should().Equal(typeof(UnorderedA), typeof(UnorderedB));
  }

  [Fact]
  public void Order_ShouldTreatAnUndeclaredOrderAsZero()
  {
    // Arrange
    DataSeed[] seeds = [new FirstSeed(), new UnorderedA()];

    // Act
    DataSeed[] ordered = [.. Seeder.Order(seeds)];

    // Assert
    // UnorderedA declares nothing, so it counts as 0 and runs before FirstSeed, which declares 1.
    ordered.Select(seed => seed.GetType()).Should().Equal(typeof(UnorderedA), typeof(FirstSeed));
  }

  #endregion

  #region Seed

  [Fact]
  public void Seed_ShouldEnqueueATransactionalExecution()
  {
    // Arrange
    Seeder sut = CreateSut();

    // Act
    sut.Seed<DiscoverableEntity>([new DiscoverableEntity { Id = Guid.NewGuid() }]);

    // Assert
    // Seeding writes several documents: it is only atomic inside a transaction.
    _stateManager.Verify(
      manager => manager.AddExecution(It.IsAny<Execution>(), true),
      Times.Once);
  }

  [Fact]
  public void Seed_ShouldEnqueueNothing_WhenThereIsNothingToSeed()
  {
    // Arrange
    Seeder sut = CreateSut();

    // Act
    sut.Seed<DiscoverableEntity>([]);

    // Assert
    _stateManager.Verify(
      manager => manager.AddExecution(It.IsAny<Execution>(), It.IsAny<bool>()),
      Times.Never);
  }

  [Fact]
  public void Seed_ShouldThrow_WhenEntitiesIsNull()
  {
    // Arrange
    Seeder sut = CreateSut();

    // Act
    Action act = () => sut.Seed<DiscoverableEntity>(null!);

    // Assert
    act.Should().Throw<ArgumentNullException>();
  }

  #endregion

  #region Apply

  [Fact]
  public void Apply_ShouldCommitOnce_ForAllSeeds()
  {
    // Arrange
    Seeder sut = CreateSut();

    // Act
    sut.Apply([new EntitySeed(), new EntitySeed()]);

    // Assert
    // Both seeds are enqueued, then committed together: an interrupted seeding must not
    // leave the database half populated.
    _stateManager.Verify(manager => manager.AddExecution(It.IsAny<Execution>(), true), Times.Exactly(2));
    _stateManager.Verify(manager => manager.Save(), Times.Once);
  }

  [Fact]
  public async Task ApplyAsync_ShouldCommitOnce_ForAllSeeds()
  {
    // Arrange
    Seeder sut = CreateSut();

    // Act
    await sut.ApplyAsync([new EntitySeed()]);

    // Assert
    _stateManager.Verify(manager => manager.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public void Apply_ShouldThrow_WhenSeedsIsNull()
  {
    // Arrange
    Seeder sut = CreateSut();

    // Act
    Action act = () => sut.Apply(null!);

    // Assert
    act.Should().Throw<ArgumentNullException>();
  }

  #endregion

  [SeedOrder(1)]
  private sealed class FirstSeed : DataSeed
  {
    public override void Seed(ISeeder seeder) { }
  }

  [SeedOrder(2)]
  private sealed class SecondSeed : DataSeed
  {
    public override void Seed(ISeeder seeder) { }
  }

  [SeedOrder(3)]
  private sealed class ThirdSeed : DataSeed
  {
    public override void Seed(ISeeder seeder) { }
  }

  private sealed class UnorderedA : DataSeed
  {
    public override void Seed(ISeeder seeder) { }
  }

  private sealed class UnorderedB : DataSeed
  {
    public override void Seed(ISeeder seeder) { }
  }

  private sealed class EntitySeed : DataSeed
  {
    public override void Seed(ISeeder seeder) =>
      seeder.Seed<DiscoverableEntity>([new DiscoverableEntity { Id = Guid.NewGuid() }]);
  }
}
