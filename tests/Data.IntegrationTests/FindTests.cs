using CodeArchitects.Platform.Data.Fixtures;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data;

[Collection(TestCollection.Name)]
public class FindTests : TestBase
{
  private readonly Customer _customer;
  private readonly Category _category;
  private readonly Action<ISeeder> _seedingAction;

  public FindTests(TestFixture fixture, ITestOutputHelper output)
    : base(fixture, output)
  {
    _customer = Customer.One();
    _customer.Address = Address.One();
    _customer.Carts = Cart.Many(2);
    _customer.Carts[0].Items = CartItem.Many(2, _customer.Carts[0].Id);
    _customer.Carts[1].Items = CartItem.Many(1, _customer.Carts[1].Id);
    _customer.Carts[0].Items![0].ShippingAddress = ShippingAddress.One();
    _customer.Carts[0].Items![1].ShippingAddress = ShippingAddress.One();
    _customer.Carts[1].Items![0].ShippingAddress = ShippingAddress.One();
    _customer.Claims = CustomerClaim.Many(2);

    _category = Category.One();
    _category.Typologies = new()
    {
      Typology.One(),
      Typology.One()
    };

    _seedingAction = seeder =>
    {
      seeder.Seed(_customer);
      seeder.Seed(_category);
    };
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnNull_WhenEntityDoesNotExists(RepositoryDependencies dependencies)
  {
    // Arrange
    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Cart, Guid>();

    // Act
    Cart? fromDb = await sut.FindAsync(Guid.NewGuid());

    // Assert
    fromDb.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntity_WhenEntityExists(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _customer.Carts![0];

    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(_seedingAction);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Cart, Guid>();

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().BeNull();
    fromDb.Customer.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntity_WhenEntityHasCompositeKey(RepositoryDependencies dependencies)
  {
    // Arrange
    CartItem item = _customer.Carts![1].Items![0];

    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(_seedingAction);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<CartItem, (int, Guid)>();

    // Act
    CartItem? fromDb = await sut.FindAsync(item.Index, item.CartId);
  
    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Index.Should().Be(item.Index);
    fromDb!.CartId.Should().Be(item.CartId);
    fromDb.Cart.Should().BeNull();
    fromDb.ShippingAddress.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeReference(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _customer.Carts![0];

    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(_seedingAction);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Cart, Guid>();

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id, _ => _
      .Include(e => e.Customer));
  
    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().BeNull();
    fromDb.Customer.Should().NotBeNull();
    fromDb.Customer!.Id.Should().Be(_customer.Id);
    fromDb.Customer.Address.Should().BeNull();
    fromDb.Customer.Claims.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeOneToMany(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _customer.Carts![0];

    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(_seedingAction);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Cart, Guid>();

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id, _ => _
      .Include(e => e.Items));

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().NotBeNull()
      .And.HaveCount(2)
      .And.Contain(item => item.Index == cart.Items![0].Index && item.CartId == cart.Id && item.ShippingAddress == null)
      .And.Contain(item => item.Index == cart.Items![1].Index && item.CartId == cart.Id && item.ShippingAddress == null);
    fromDb.Customer.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeManyToMany(RepositoryDependencies dependencies)
  {
    // Arrange
    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(_seedingAction);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Category, Guid>();

    // Act
    Category? fromDb = await sut.FindAsync(_category.Id, _ => _
      .Include(e => e.Typologies));

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(_category.Id);
    fromDb.Typologies.Should().NotBeNull()
      .And.HaveCount(2)
      .And.Contain(typology => typology.Id == _category.Typologies![0].Id)
      .And.Contain(typology => typology.Id == _category.Typologies![1].Id);
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeReferenceThenReference(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _customer.Carts![0];

    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(_seedingAction);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Cart, Guid>();

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id, _ => _
      .Include(e => e.Customer, _ => _
        .Include(e => e.Address)));

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().BeNull();
    fromDb.Customer.Should().NotBeNull();
    fromDb.Customer!.Id.Should().Be(_customer.Id);
    fromDb.Customer.Claims.Should().BeNull();
    fromDb.Customer.Address.Should().NotBeNull();
    fromDb.Customer.Address!.Id.Should().Be(_customer.Address!.Id);
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeReferenceThenCollection(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _customer.Carts![0];

    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(_seedingAction);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Cart, Guid>();

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id, _ => _
      .Include(e => e.Customer, _ => _
        .Include(e => e.Claims)));

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().BeNull();
    fromDb.Customer.Should().NotBeNull();
    fromDb.Customer!.Id.Should().Be(_customer.Id);
    fromDb.Customer.Address.Should().BeNull();
    fromDb.Customer.Claims.Should().HaveCount(2)
      .And.ContainSingle(claim => claim.Id == _customer.Claims![0].Id)
      .And.ContainSingle(claim => claim.Id == _customer.Claims![1].Id);
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeCollectionThenReference(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _customer.Carts![0];

    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(_seedingAction);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Cart, Guid>();

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id, _ => _
      .Include(e => e.Items, _ => _
        .Include(e => e.ShippingAddress)));

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().NotBeNull()
      .And.HaveCount(2)
      .And.Contain(item => item.Index == cart.Items![0].Index && item.CartId == cart.Id && item.ShippingAddress!.Id == cart.Items[0].ShippingAddress!.Id)
      .And.Contain(item => item.Index == cart.Items![1].Index && item.CartId == cart.Id && item.ShippingAddress!.Id == cart.Items[1].ShippingAddress!.Id);
    fromDb.Customer.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeCollectionThenCollection(RepositoryDependencies dependencies)
  {
    // Arrange
    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(_seedingAction);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Customer, Guid>();

    // Act
    Customer? fromDb = await sut.FindAsync(_customer.Id, _ => _
      .Include(e => e.Carts, _ => _
        .Include(e => e.Items)));

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(_customer.Id);
    fromDb.Carts.Should().NotBeNull().And.HaveCount(2);
    fromDb.Carts.Should().Contain(cart => cart.Id == _customer.Carts![0].Id)
      .Which.Items.Should().HaveCount(2)
        .And.Contain(item => item.Index == _customer.Carts![0].Items![0].Index && item.CartId == _customer.Carts![0].Id && item.ShippingAddress == null)
        .And.Contain(item => item.Index == _customer.Carts![0].Items![1].Index && item.CartId == _customer.Carts![0].Id && item.ShippingAddress == null);
    fromDb.Carts.Should().Contain(cart => cart.Id == _customer.Carts![1].Id)
      .Which.Items.Should().HaveCount(1)
        .And.Contain(item => item.Index == _customer.Carts![1].Items![0].Index && item.CartId == _customer.Carts![1].Id && item.ShippingAddress == null);
    fromDb.Address.Should().BeNull();
    fromDb.Claims.Should().BeNull();
  }
}
