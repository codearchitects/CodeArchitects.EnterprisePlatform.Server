using CodeArchitects.Platform.Data.Fixtures;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data;

[Collection(TestCollection.Name)]
public class FindAsyncTests : TestBase
{
  private readonly User _user;
  private readonly Category _category;
  private readonly IEnumerable<object> _seed;

  public FindAsyncTests(TestFixture fixture, ITestOutputHelper output)
    : base(fixture, output)
  {
    _user = User.One();
    _user.Address = Address.One();
    _user.Carts = Cart.Many(2);
    _user.Carts[0].Items = CartItem.Many(2, _user.Carts[0].Id);
    _user.Carts[1].Items = CartItem.Many(1, _user.Carts[1].Id);
    _user.Carts[0].Items![0].ShippingAddress = ShippingAddress.One();
    _user.Carts[0].Items![1].ShippingAddress = ShippingAddress.One();
    _user.Carts[1].Items![0].ShippingAddress = ShippingAddress.One();
    _user.Claims = UserClaim.Many(2);

    _category = Category.One();
    _category.Typologies = new()
    {
      Typology.One(),
      Typology.One()
    };

    _seed = new object[] { _user, _category };
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnNull_WhenEntityDoesNotExists(RepositoryDependencies dependencies)
  {
    // Arrange
    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies);

    // Act
    Cart? fromDb = await sut.FindAsync(Guid.NewGuid());

    // Assert
    fromDb.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntity_WhenEntityExists(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _user.Carts![0];

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, _seed);

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().BeNull();
    fromDb.User.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntity_WhenEntityHasCompositeKey(RepositoryDependencies dependencies)
  {
    // Arrange
    CartItem item = _user.Carts![1].Items![0];

    var sut = _fixture.CreateRepository<CartItem, (int, Guid)>(dependencies, _seed);

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
    Cart cart = _user.Carts![0];

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, _seed);

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id, _ => _
      .Include(e => e.User));
  
    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().BeNull();
    fromDb.User.Should().NotBeNull();
    fromDb.User!.Id.Should().Be(_user.Id);
    fromDb.User.Address.Should().BeNull();
    fromDb.User.Claims.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeOneToMany(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _user.Carts![0];

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, _seed);

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
    fromDb.User.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeManyToMany(RepositoryDependencies dependencies)
  {
    // Arrange
    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, _seed);

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
    Cart cart = _user.Carts![0];

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, _seed);

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id, _ => _
      .Include(e => e.User, _ => _
        .Include(e => e.Address)));

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().BeNull();
    fromDb.User.Should().NotBeNull();
    fromDb.User!.Id.Should().Be(_user.Id);
    fromDb.User.Address.Should().NotBeNull();
    fromDb.User.Address!.Id.Should().Be(_user.Address!.Id);
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeReferenceThenCollection(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _user.Carts![0];

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, _seed);

    // Act
    Cart? fromDb = await sut.FindAsync(cart.Id, _ => _
      .Include(e => e.User, _ => _
        .Include(e => e.Address)));

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(cart.Id);
    fromDb.Items.Should().BeNull();
    fromDb.User.Should().NotBeNull();
    fromDb.User!.Id.Should().Be(_user.Id);
    fromDb.User.Address.Should().NotBeNull();
    fromDb.User.Address!.Id.Should().Be(_user.Address!.Id);
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeCollectionThenReference(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = _user.Carts![0];

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, _seed);

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
    fromDb.User.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntityWithCorrectNavigations_WhenIncludeCollectionThenCollection(RepositoryDependencies dependencies)
  {
    // Arrange
    var sut = _fixture.CreateRepository<User, Guid>(dependencies, _seed);

    // Act
    User? fromDb = await sut.FindAsync(_user.Id, _ => _
      .Include(e => e.Carts, _ => _
        .Include(e => e.Items)));

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(_user.Id);
    fromDb.Carts.Should().NotBeNull().And.HaveCount(2);
    fromDb.Carts.Should().Contain(cart => cart.Id == _user.Carts![0].Id)
      .Which.Items.Should().HaveCount(2)
        .And.Contain(item => item.Index == _user.Carts![0].Items![0].Index && item.CartId == _user.Carts![0].Id && item.ShippingAddress == null)
        .And.Contain(item => item.Index == _user.Carts![0].Items![1].Index && item.CartId == _user.Carts![0].Id && item.ShippingAddress == null);
    fromDb.Carts.Should().Contain(cart => cart.Id == _user.Carts![1].Id)
      .Which.Items.Should().HaveCount(1)
        .And.Contain(item => item.Index == _user.Carts![1].Items![0].Index && item.CartId == _user.Carts![1].Id && item.ShippingAddress == null);
    fromDb.Address.Should().BeNull();
    fromDb.Claims.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntity_WhenExistsAndBelogsToCurrentTenant(RepositoryDependencies dependencies)
  {
    // Arrange
    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();

    var sut = _fixture.CreateRepository<TenantEntity, Guid>(dependencies, new[] { entity });

    // Act
    TenantEntity? fromDb = await sut.FindAsync(entity.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(entity.Id);
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnNull_WhenExistsButBelongsToAnotherTenant(RepositoryDependencies dependencies)
  {
    // Arrange
    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();

    var sut = _fixture.CreateRepository<TenantEntity, Guid>(dependencies, new[] { entity });

    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    // Act
    TenantEntity? fromDb = await sut.FindAsync(entity.Id);

    // Assert
    fromDb.Should().BeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnEntity_WhenExistsAndIsNotSoftDeleted(RepositoryDependencies dependencies)
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();

    var sut = _fixture.CreateRepository<SoftDeleteEntity, Guid>(dependencies, new[] { entity });

    // Act
    SoftDeleteEntity? fromDb = await sut.FindAsync(entity.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(entity.Id);
  }

  [Theory, RepositoryDependenciesData]
  public async Task FindAsync_ShouldReturnNull_WhenExistsButIsSoftDeleted(RepositoryDependencies dependencies)
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();

    var sut = _fixture.CreateRepository<SoftDeleteEntity, Guid>(dependencies, new[] { entity });

    _fixture.SoftDelete(entity);

    // Act
    SoftDeleteEntity? fromDb = await sut.FindAsync(entity.Id);

    // Assert
    fromDb.Should().BeNull();
  }
}
