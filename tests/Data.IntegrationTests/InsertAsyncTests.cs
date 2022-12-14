using CodeArchitects.Platform.Data.Fixtures;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data;

[Collection(TestCollection.Name)]
public class InsertAsyncTests : TestBase
{
  public InsertAsyncTests(TestFixture fixture, ITestOutputHelper output)
    : base(fixture, output)
  {
  }

  [Theory, RepositoryDependenciesData]
  public async Task InsertAsync_ShouldInsertEntityAndAddLinks_WhenNavigationIsManyToMany(RepositoryDependencies dependencies)
  {
    // Arrange
    Typology typology = Typology.One();
    Category category = Category.One();
    category.Typologies = new() { typology };

    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, new[] { typology });

    // Act
    await sut.InsertAsync(category);

    Category? fromDb = _fixture.DbContext.Set<Category>()
      .Include(x => x.Typologies)
      .FirstOrDefault(x => x.Id == category.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Typologies.Should().HaveCount(1)
      .And.ContainSingle(t => t.Id == typology.Id && t.Name == typology.Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task InsertAsync_ShouldInsertEntityAndAddLinks_WhenNavigationIsOneToManyComposition(RepositoryDependencies dependencies)
  {
    // Arrange
    User user = User.One();
    Cart cart = Cart.One();
    cart.Items = CartItem.Many(1, cart.Id);
    user.Carts = new() { cart };

    var sut = _fixture.CreateRepository<User, Guid>(dependencies, new[] { cart });

    // Act
    await sut.InsertAsync(user);

    User? fromDb = _fixture.DbContext.Set<User>()
      .Include(x => x.Carts)
      .FirstOrDefault(x => x.Id == user.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Carts.Should().HaveCount(1)
      .And.ContainSingle(c => c.Id == cart.Id && c.Name == cart.Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task InsertAsync_ShouldInsertEntityAndAddLinks_WhenNavigationIsOneToOneComposition(RepositoryDependencies dependencies)
  {
    // Arrange
    Person first = Person.One();
    Person second = Person.One();
    second.Partner = first;

    var sut = _fixture.CreateRepository<Person, Guid>(dependencies, new[] { first });

    // Act
    await sut.InsertAsync(second);

    Person? fromDb = _fixture.DbContext.Set<Person>()
      .Include(x => x.Partner)
      .FirstOrDefault(x => x.Id == second.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Partner.Should().NotBeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task InsertAsync_ShouldInsertEntityAndChildren_WhenNavigationIsOneToManyAggregation(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = Cart.One();
    CartItem item = CartItem.One(cart.Id);
    cart.Items = new() { item };

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies);

    // Act
    await sut.InsertAsync(cart);

    Cart? fromDb = _fixture.DbContext.Set<Cart>()
      .Include(x => x.Items)
      .FirstOrDefault(x => x.Id == cart.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Items.Should().HaveCount(1)
      .And.ContainSingle(i => i.Index == item.Index && i.CartId == item.CartId && i.Name == item.Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task InsertAsync_ShouldInsertEntityAndChild_WhenNavigationIsOneToOneAggregation(RepositoryDependencies dependencies)
  {
    // Arrange
    User user = User.One();
    user.Address = Address.One();

    var sut = _fixture.CreateRepository<User, Guid>(dependencies);

    // Act
    await sut.InsertAsync(user);

    User? fromDb = _fixture.DbContext.Set<User>()
      .Include(x => x.Address)
      .FirstOrDefault(x => x.Id == user.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Address.Should().NotBeNull();
  }

  [MultitenancyTest]
  [Theory, RepositoryDependenciesData]
  public async Task InsertAsync_ShouldInsertEntityWithCurrentTenantId(RepositoryDependencies dependencies)
  {
    // Arrange
    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();

    var sut = _fixture.CreateRepository<TenantEntity, Guid>(dependencies);
    
    // Act
    await sut.InsertAsync(entity);

    // Assert
    TenantEntity? fromDb = _fixture.DbContext.Set<TenantEntity>()
      .FirstOrDefault(e => e.Id == entity.Id);

    fromDb.Should().NotBeNull();
    _fixture.DbContext.Entry(fromDb!).Property(TenantEntity.TenantIdPropertyName).CurrentValue.Should().Be(_fixture.MultitenancyContext.TenantId);
  }
}
