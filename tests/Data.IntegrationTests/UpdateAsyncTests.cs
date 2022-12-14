using CodeArchitects.Platform.Data.Fixtures;
using CodeArchitects.Platform.Data.Fixtures.Model;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data;

[Collection(TestCollection.Name)]
public class UpdateAsyncTests : TestBase
{
  public UpdateAsyncTests(TestFixture fixture, ITestOutputHelper output)
    : base(fixture, output)
  {
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityOnly_WhenNavigationIsManyToMany(RepositoryDependencies dependencies)
  {
    // Arrange
    Category category = Category.One();
    Typology typology0 = Typology.One();
    Typology typology1 = Typology.One();
    category.Typologies = new() { typology0, typology1 };
    string? oldTypologyName = typology0.Name;

    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, new object[] { typology0, typology1, category });

    category.Name = "New category name";
    typology0.Name = "New typology name";

    // Act
    await sut.UpdateAsync(category);

    // Assert
    Category? fromDb = _fixture.DbContext.Set<Category>()
      .Include(x => x.Typologies)
      .FirstOrDefault(x => x.Id == category.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(category.Name);
    fromDb.Typologies.Should().HaveCount(2)
      .And.ContainSingle(typology => typology.Id == typology0.Id && typology.Name == oldTypologyName)
      .And.ContainSingle(typology => typology.Id == typology1.Id && typology.Name == typology1.Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndAddLinks_WhenNavigationIsManyToManyAndNavigationIsAdded(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    Category category = Category.One();
    Typology typology0 = Typology.One();
    Typology typology1 = Typology.One();
    category.Typologies = new() { typology0 };
    string? oldTypologyName = typology0.Name;

    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, new object[] { typology0, typology1, category });

    category.Name = "New category name";
    typology0.Name = "New typology name";
    category.Typologies.Add(typology1);
    trackingContext.SetTrackingState(category, TrackingState.Modified);
    trackingContext.SetTrackingState(typology0, TrackingState.Unchanged);
    trackingContext.SetTrackingState(typology1, TrackingState.Added);

    // Act
    await sut.UpdateAsync(category);

    // Assert
    Category? fromDb = _fixture.DbContext.Set<Category>()
      .Include(x => x.Typologies)
      .FirstOrDefault(x => x.Id == category.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(category.Name);
    fromDb.Typologies.Should().HaveCount(2)
      .And.ContainSingle(typology => typology.Id == typology0.Id && typology.Name == oldTypologyName)
      .And.ContainSingle(typology => typology.Id == typology1.Id && typology.Name == typology1.Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndRemoveLinks_WhenNavigationIsManyToManyAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    Category category = Category.One();
    Typology typology0 = Typology.One();
    Typology typology1 = Typology.One();
    category.Typologies = new() { typology0, typology1 };
    string? oldTypologyName = typology0.Name;

    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, new object[] { typology0, typology1, category });

    category.Name = "New category name";
    typology0.Name = "New typology name";
    trackingContext.SetTrackingState(category, TrackingState.Modified);
    trackingContext.SetTrackingState(typology0, TrackingState.Unchanged);
    trackingContext.SetTrackingState(typology1, TrackingState.Removed);

    // Act
    await sut.UpdateAsync(category);

    // Assert
    Category? fromDb = _fixture.DbContext.Set<Category>()
      .Include(x => x.Typologies)
      .FirstOrDefault(x => x.Id == category.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(category.Name);
    fromDb.Typologies.Should().HaveCount(1)
      .And.ContainSingle(typology => typology.Id == typology0.Id && typology.Name == oldTypologyName);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldThrow_WhenNavigationIsManyToManyAndNavigationIsModified(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    Category category = Category.One();
    Typology typology0 = Typology.One();
    Typology typology1 = Typology.One();
    category.Typologies = new() { typology0, typology1 };

    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, new object[] { typology0, typology1, category });

    trackingContext.SetTrackingState(category, TrackingState.Modified);
    trackingContext.SetTrackingState(typology0, TrackingState.Modified);
    trackingContext.SetTrackingState(typology1, TrackingState.Modified);

    // Act
    Func<Task> act = () => sut.UpdateAsync(category);

    // Assert
    await act.Should().ThrowAsync<InvalidTrackingStateException>();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityOnly_WhenNavigationIsOneToManyComposition(RepositoryDependencies dependencies)
  {
    // Arrange
    User user = User.One();
    Cart cart0 = Cart.One();
    Cart cart1 = Cart.One();
    user.Carts = new() { cart0, cart1 };
    string? oldCartName = cart0.Name;

    var sut = _fixture.CreateRepository<User, Guid>(dependencies, new object[] { cart0, cart1, user });

    user.Name = "New user name";
    cart0.Name = "New cart name";

    // Act
    await sut.UpdateAsync(user);

    // Assert
    User? fromDb = _fixture.DbContext.Set<User>()
      .Include(x => x.Carts)
      .FirstOrDefault(x => x.Id == user.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(user.Name);
    fromDb.Carts.Should().HaveCount(2)
      .And.ContainSingle(cart => cart.Id == cart0.Id && cart.Name == oldCartName)
      .And.ContainSingle(cart => cart.Id == cart1.Id && cart.Name == cart1.Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndAddLinks_WhenNavigationIsOneToManyCompositionAndNavigationIsAdded(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    User user = User.One();
    Cart cart0 = Cart.One();
    Cart cart1 = Cart.One();
    user.Carts = new() { cart0 };
    string? oldCartName = cart0.Name;

    var sut = _fixture.CreateRepository<User, Guid>(dependencies, new object[] { cart0, cart1, user });

    user.Name = "New user name";
    cart0.Name = "New cart name";
    user.Carts.Add(cart1);
    trackingContext.SetTrackingState(user, TrackingState.Modified);
    trackingContext.SetTrackingState(cart0, TrackingState.Unchanged);
    trackingContext.SetTrackingState(cart1, TrackingState.Added);

    // Act
    await sut.UpdateAsync(user);

    // Assert
    User? fromDb = _fixture.DbContext.Set<User>()
      .Include(x => x.Carts)
      .FirstOrDefault(x => x.Id == user.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(user.Name);
    fromDb.Carts.Should().HaveCount(2)
      .And.ContainSingle(cart => cart.Id == cart0.Id && cart.Name == oldCartName)
      .And.ContainSingle(cart => cart.Id == cart1.Id && cart.Name == cart1.Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndRemoveLinks_WhenNavigationIsOneToManyCompositionAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    User user = User.One();
    Cart cart0 = Cart.One();
    Cart cart1 = Cart.One();
    user.Carts = new() { cart0, cart1 };
    string? oldCartName = cart0.Name;

    var sut = _fixture.CreateRepository<User, Guid>(dependencies, new object[] { cart0, cart1, user });

    user.Name = "New user name";
    cart0.Name = "New cart name";
    trackingContext.SetTrackingState(user, TrackingState.Modified);
    trackingContext.SetTrackingState(cart0, TrackingState.Unchanged);
    trackingContext.SetTrackingState(cart1, TrackingState.Removed);

    // Act
    await sut.UpdateAsync(user);

    // Assert
    User? fromDb = _fixture.DbContext.Set<User>()
      .Include(x => x.Carts)
      .FirstOrDefault(x => x.Id == user.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(user.Name);
    fromDb.Carts.Should().HaveCount(1)
      .And.ContainSingle(cart => cart.Id == cart0.Id && cart.Name == oldCartName);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndRemoveOrAddLinksOfAggregateEntity_WhenNavigationIsManyToManyAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    Product product1 = Product.One();
    Product product2 = Product.One();
    Product product3 = Product.One();
    Cart cart = Cart.One();
    CartItem item = CartItem.One(cart.Id);
    item.Products = new() { product1, product2 };

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, new object[] { cart, item, product1, product2, product3 });

    cart.Name = "New cart name";
    item.Name = "New item name";
    item.Products = new() { product1, product3 };
    trackingContext.SetTrackingState(cart, TrackingState.Modified);
    trackingContext.SetTrackingState(item, TrackingState.Modified);
    trackingContext.SetTrackingState(product1, TrackingState.Removed);
    trackingContext.SetTrackingState(product3, TrackingState.Added);

    // Act
    await sut.UpdateAsync(cart);

    // Assert
    Cart? fromDb = _fixture.DbContext.Set<Cart>()
      .Include(x => x.Items!)
      .ThenInclude(x => x.Products)
      .FirstOrDefault(x => x.Id == cart.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(cart.Name);
    fromDb.Items.Should().HaveCount(1);
    fromDb.Items![0].Products.Should().HaveCount(2)
      .And.ContainSingle(product => product.Id == product2.Id)
      .And.ContainSingle(product => product.Id == product3.Id);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldThrow_WhenNavigationIsOneToManyCompositionAndNavigationIsModified(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    User user = User.One();
    Cart cart0 = Cart.One();
    Cart cart1 = Cart.One();
    user.Carts = new() { cart0, cart1 };

    var sut = _fixture.CreateRepository<User, Guid>(dependencies, new object[] { cart0, cart1, user });

    trackingContext.SetTrackingState(user, TrackingState.Modified);
    trackingContext.SetTrackingState(cart0, TrackingState.Modified);
    trackingContext.SetTrackingState(cart1, TrackingState.Modified);

    // Act
    Func<Task> act = () => sut.UpdateAsync(user);

    // Assert
    await act.Should().ThrowAsync<InvalidTrackingStateException>();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityOnly_WhenNavigationIsOneToOneComposition(RepositoryDependencies dependencies)
  {
    // Arrange
    Person first = Person.One();
    Person second = Person.One();
    first.Partner = second;
    string? oldSecondName = second.Name;

    var sut = _fixture.CreateRepository<Person, Guid>(dependencies, new object[] { first, second });

    first.Name = "New first name";
    second.Name = "New second name";

    // Act
    await sut.UpdateAsync(first);

    // Assert
    Person? fromDb = _fixture.DbContext.Set<Person>()
      .Include(x => x.Partner)
      .FirstOrDefault(x => x.Id == first.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(first.Name);
    fromDb.Partner!.Name.Should().Be(oldSecondName);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndAddLinks_WhenNavigationIsOneToOneCompositionAndNavigationIsAdded(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    Person first = Person.One();
    Person second = Person.One();
    string? oldSecondName = second.Name;

    var sut = _fixture.CreateRepository<Person, Guid>(dependencies, new object[] { first, second });

    first.Name = "New user name";
    second.Name = "New second name";
    first.Partner = second;
    trackingContext.SetTrackingState(first, TrackingState.Modified);
    trackingContext.SetTrackingState(second, TrackingState.Added);

    // Act
    await sut.UpdateAsync(first);

    // Assert
    Person? fromDb = _fixture.DbContext.Set<Person>()
      .Include(x => x.Partner)
      .FirstOrDefault(x => x.Id == first.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(first.Name);
    fromDb.Partner.Should().NotBeNull();
    fromDb.Partner!.Id.Should().Be(second.Id);
    fromDb.Partner.Name.Should().Be(oldSecondName);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndRemoveLinks_WhenNavigationIsOneToOneCompositionAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    Person first = Person.One();
    Person second = Person.One();
    string? oldSecondName = second.Name;

    var sut = _fixture.CreateRepository<Person, Guid>(dependencies, new object[] { first, second });

    first.Name = "New user name";
    second.Name = "New second name";
    trackingContext.SetTrackingState(first, TrackingState.Modified);
    trackingContext.SetTrackingState(second, TrackingState.Removed);

    // Act
    await sut.UpdateAsync(first);

    // Assert
    Person? fromDb = _fixture.DbContext.Set<Person>()
      .Include(x => x.Partner)
      .FirstOrDefault(x => x.Id == first.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(first.Name);
    fromDb.Partner.Should().BeNull();
    _fixture.DbContext.Set<Person>().FirstOrDefault(person => person.Id == second.Id).Should().NotBeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldThrow_WhenNavigationIsOneToOneCompositionAndNavigationIsModified(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    Person first = Person.One();
    Person second = Person.One();
    first.Partner = second;

    var sut = _fixture.CreateRepository<Person, Guid>(dependencies, new object[] { first, second });

    trackingContext.SetTrackingState(first, TrackingState.Modified);
    trackingContext.SetTrackingState(second, TrackingState.Modified);

    // Act
    Func<Task> act = () => sut.UpdateAsync(first);

    // Assert
    await act.Should().ThrowAsync<InvalidTrackingStateException>();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndAddOrModifyOrDeleteChildren_WhenNavigationIsOneToManyAggregation(RepositoryDependencies dependencies)
  {
    // Arrange
    ITrackingContext trackingContext = dependencies.TrackingContext;

    Cart cart = Cart.One();
    CartItem item0 = CartItem.One(cart.Id);
    CartItem item1 = CartItem.One(cart.Id);
    cart.Items = new() { item0, item1 };

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, new object[] { cart, item0, item1 });

    cart.Name = "New cart name";
    item1.Name = "New item name";
    CartItem item2 = CartItem.One(cart.Id);
    cart.Items.Add(item2);
    trackingContext.SetTrackingState(cart, TrackingState.Modified);
    trackingContext.SetTrackingState(item0, TrackingState.Removed);
    trackingContext.SetTrackingState(item1, TrackingState.Modified);
    trackingContext.SetTrackingState(item2, TrackingState.Added);

    // Act
    await sut.UpdateAsync(cart);

    // Assert
    Cart? fromDb = _fixture.DbContext.Set<Cart>()
      .Include(x => x.Items)
      .FirstOrDefault(x => x.Id == cart.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(cart.Name);
    fromDb.Items.Should().HaveCount(2)
      .And.ContainSingle(item => item.Index == item1.Index && item.Name == item1.Name)
      .And.ContainSingle(item => item.Index == item2.Index && item.Name == item2.Name);
  }

  [MultitenancyTest]
  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityBelongsToCurrentTenant(RepositoryDependencies dependencies)
  {
    // Arrange
    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();

    var sut = _fixture.CreateRepository<TenantEntity, Guid>(dependencies, new[] { entity });

    entity.Name = "New entity name";

    // Act
    await sut.UpdateAsync(entity);

    // Assert
    TenantEntity? fromDb = _fixture.DbContext.Set<TenantEntity>()
      .FirstOrDefault(e => e.Id == entity.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(entity.Name);
  }

  [MultitenancyTest]
  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldThrow_WhenEntityBelongsToAnotherTenant(RepositoryDependencies dependencies)
  {
    // Arrange
    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();

    var sut = _fixture.CreateRepository<TenantEntity, Guid>(dependencies, new[] { entity });

    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    // Act
    Func<Task> act = () => sut.UpdateAsync(entity);

    // Assert
    await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
  }

  [SoftDeleteTest]
  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityIsNotSoftDeleted(RepositoryDependencies dependencies)
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();

    var sut = _fixture.CreateRepository<SoftDeleteEntity, Guid>(dependencies, new[] { entity });

    entity.Name = "New entity name";

    // Act
    await sut.UpdateAsync(entity);

    // Assert
    _fixture.SoftDeleteContext.ShouldFilter = false;
    SoftDeleteEntity? fromDb = _fixture.DbContext.Set<SoftDeleteEntity>().FirstOrDefault(e => e.Id == entity.Id);
    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(entity.Name);
  }

  [SoftDeleteTest]
  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldThrow_WhenEntityIsSoftDeleted(RepositoryDependencies dependencies)
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();

    var sut = _fixture.CreateRepository<SoftDeleteEntity, Guid>(dependencies, new[] { entity });

    _fixture.SoftDelete(entity);

    // Act
    Func<Task> act = () => sut.UpdateAsync(entity);

    // Assert
    await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
  }
}
