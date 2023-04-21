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
    List<Typology> typologies = Typology.Many(2);
    category.Typologies = new(typologies);
    string? oldTypologyName = typologies[0].Name;

    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, seeder =>
    {
      seeder.Seed(typologies);
      seeder.Seed(category);
    });

    category.Name = "New category name";
    typologies[0].Name = "New typology name";

    // Act
    await sut.UpdateAsync(category);

    // Assert
    Category? fromDb = _fixture.DbContext.Set<Category>()
      .Include(x => x.Typologies)
      .FirstOrDefault(x => x.Id == category.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(category.Name);
    fromDb.Typologies.Should().HaveCount(2)
      .And.ContainSingle(typology => typology.Id == typologies[0].Id && typology.Name == oldTypologyName)
      .And.ContainSingle(typology => typology.Id == typologies[1].Id && typology.Name == typologies[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndAddLinks_WhenNavigationIsManyToManyAndNavigationIsAdded(RepositoryDependencies dependencies)
  {
    // Arrange
    Category category = Category.One();
    List<Typology> typologies = Typology.Many(2);
    category.Typologies = new() { typologies[0] };
    string? oldTypologyName = typologies[0].Name;

    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, seeder =>
    {
      seeder.Seed(typologies);
      seeder.Seed(category);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    category.Name = "New category name";
    typologies[0].Name = "New typology name";
    category.Typologies.Add(typologies[1]);
    trackingContext.SetTrackingState(typologies[0], TrackingState.Unchanged);
    trackingContext.SetTrackingState(typologies[1], TrackingState.Added);

    // Act
    await sut.UpdateAsync(category);

    // Assert
    Category? fromDb = _fixture.DbContext.Set<Category>()
      .Include(x => x.Typologies)
      .FirstOrDefault(x => x.Id == category.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(category.Name);
    fromDb.Typologies.Should().HaveCount(2)
      .And.ContainSingle(typology => typology.Id == typologies[0].Id && typology.Name == oldTypologyName)
      .And.ContainSingle(typology => typology.Id == typologies[1].Id && typology.Name == typologies[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndRemoveLinks_WhenNavigationIsManyToManyAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    Category category = Category.One();
    List<Typology> typologies = Typology.Many(2);
    category.Typologies = new(typologies);
    string? oldTypologyName = typologies[0].Name;

    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, seeder =>
    {
      seeder.Seed(typologies);
      seeder.Seed(category);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    category.Name = "New category name";
    typologies[0].Name = "New typology name";
    trackingContext.SetTrackingState(typologies[0], TrackingState.Unchanged);
    trackingContext.SetTrackingState(typologies[1], TrackingState.Removed);

    // Act
    await sut.UpdateAsync(category);

    // Assert
    Category? fromDb = _fixture.DbContext.Set<Category>()
      .Include(x => x.Typologies)
      .FirstOrDefault(x => x.Id == category.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(category.Name);
    fromDb.Typologies.Should().HaveCount(1)
      .And.ContainSingle(typology => typology.Id == typologies[0].Id && typology.Name == oldTypologyName);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldThrow_WhenNavigationIsManyToManyAndNavigationIsModified(RepositoryDependencies dependencies)
  {
    // Arrange
    Category category = Category.One();
    List<Typology> typologies = Typology.Many(2);
    category.Typologies = new(typologies);

    var sut = _fixture.CreateRepository<Category, Guid>(dependencies, seeder =>
    {
      seeder.Seed(typologies);
      seeder.Seed(category);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    trackingContext.SetTrackingState(typologies[0], TrackingState.Modified);
    trackingContext.SetTrackingState(typologies[1], TrackingState.Modified);

    // Act
    Func<Task> act = () => sut.UpdateAsync(category);

    // Assert
    await act.Should().ThrowAsync<InvalidTrackingStateException>();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityOnly_WhenNavigationIsOneToManyIntraAggregate(RepositoryDependencies dependencies)
  {
    // Arrange
    Customer customer = Customer.One();
    List<Cart> carts = Cart.Many(2);
    customer.Carts = new(carts);
    string? oldCartName = carts[0].Name;

    var sut = _fixture.CreateRepository<Customer, Guid>(dependencies, seeder =>
    {
      seeder.Seed(carts);
      seeder.Seed(customer);
    });

    customer.Name = "New customer name";
    carts[0].Name = "New cart name";

    // Act
    await sut.UpdateAsync(customer);

    // Assert
    Customer? fromDb = _fixture.DbContext.Set<Customer>()
      .Include(x => x.Carts)
      .FirstOrDefault(x => x.Id == customer.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(customer.Name);
    fromDb.Carts.Should().HaveCount(2)
      .And.ContainSingle(cart => cart.Id == carts[0].Id && cart.Name == oldCartName)
      .And.ContainSingle(cart => cart.Id == carts[1].Id && cart.Name == carts[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndAddLinks_WhenNavigationIsOneToManyInterAggregateAndNavigationIsAdded(RepositoryDependencies dependencies)
  {
    // Arrange
    Customer customer = Customer.One();
    List<Cart> carts = Cart.Many(2);
    customer.Carts = new() { carts[0] };
    string? oldCartName = carts[0].Name;

    var sut = _fixture.CreateRepository<Customer, Guid>(dependencies, seeder =>
    {
      seeder.Seed(carts);
      seeder.Seed(customer);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    customer.Name = "New customer name";
    carts[0].Name = "New cart name";
    customer.Carts.Add(carts[1]);
    trackingContext.SetTrackingState(carts[0], TrackingState.Unchanged);
    trackingContext.SetTrackingState(carts[1], TrackingState.Added);

    // Act
    await sut.UpdateAsync(customer);

    // Assert
    Customer? fromDb = _fixture.DbContext.Set<Customer>()
      .Include(x => x.Carts)
      .FirstOrDefault(x => x.Id == customer.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(customer.Name);
    fromDb.Carts.Should().HaveCount(2)
      .And.ContainSingle(cart => cart.Id == carts[0].Id && cart.Name == oldCartName)
      .And.ContainSingle(cart => cart.Id == carts[1].Id && cart.Name == carts[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndRemoveLinks_WhenNavigationIsOneToManyInterAggregateAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    Customer customer = Customer.One();
    List<Cart> carts = Cart.Many(2);
    customer.Carts = new(carts);
    string? oldCartName = carts[0].Name;

    var sut = _fixture.CreateRepository<Customer, Guid>(dependencies, seeder =>
    {
      seeder.Seed(carts);
      seeder.Seed(customer);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    customer.Name = "New customer name";
    carts[0].Name = "New cart name";
    trackingContext.SetTrackingState(carts[0], TrackingState.Unchanged);
    trackingContext.SetTrackingState(carts[1], TrackingState.Removed);

    // Act
    await sut.UpdateAsync(customer);

    // Assert
    Customer? fromDb = _fixture.DbContext.Set<Customer>()
      .Include(x => x.Carts)
      .FirstOrDefault(x => x.Id == customer.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(customer.Name);
    fromDb.Carts.Should().HaveCount(1)
      .And.ContainSingle(cart => cart.Id == carts[0].Id && cart.Name == oldCartName);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndRemoveOrAddLinksOfAggregateEntity_WhenNavigationIsManyToManyAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Product> products = Product.Many(3);
    Cart cart = Cart.One();
    CartItem item = CartItem.One(cart.Id);
    item.Products = new() { products[0], products[1] };
    cart.Items = new() { item };

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, seeder =>
    {
      seeder.Seed(cart);
      seeder.Seed(item);
      seeder.Seed(products);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    cart.Name = "New cart name";
    item.Name = "New item name";
    item.Products = new() { products[0], products[2] };
    trackingContext.SetTrackingState(item, TrackingState.Modified);
    trackingContext.SetTrackingState(products[0], TrackingState.Removed);
    trackingContext.SetTrackingState(products[2], TrackingState.Added);

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
      .And.ContainSingle(product => product.Id == products[1].Id)
      .And.ContainSingle(product => product.Id == products[2].Id);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldThrow_WhenNavigationIsOneToManyInterAggregateAndNavigationIsModified(RepositoryDependencies dependencies)
  {
    // Arrange
    Customer customer = Customer.One();
    List<Cart> carts = Cart.Many(2);
    customer.Carts = new(carts);

    var sut = _fixture.CreateRepository<Customer, Guid>(dependencies, seeder =>
    {
      seeder.Seed(carts);
      seeder.Seed(customer);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    trackingContext.SetTrackingState(carts[0], TrackingState.Modified);
    trackingContext.SetTrackingState(carts[1], TrackingState.Modified);

    // Act
    Func<Task> act = () => sut.UpdateAsync(customer);

    // Assert
    await act.Should().ThrowAsync<InvalidTrackingStateException>();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityOnly_WhenNavigationIsOneToOneInterAggregate(RepositoryDependencies dependencies)
  {
    // Arrange
    Person first = Person.One();
    Person second = Person.One();
    first.Partner = second;
    string? oldSecondName = second.Name;

    var sut = _fixture.CreateRepository<Person, Guid>(dependencies, seeder =>
    {
      seeder.Seed(first);
      seeder.Seed(second);
    }, seedImplementation: RepositoryImplementation.EFCore);

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
  public async Task UpdateAsync_ShouldUpdateEntityAndAddLinks_WhenNavigationIsOneToOneInterAggregateAndNavigationIsAdded(RepositoryDependencies dependencies)
  {
    // Arrange
    Person first = Person.One();
    Person second = Person.One();
    string? oldSecondName = second.Name;

    var sut = _fixture.CreateRepository<Person, Guid>(dependencies, seeder =>
    {
      seeder.Seed(first);
      seeder.Seed(second);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    first.Name = "New first name";
    second.Name = "New second name";
    first.Partner = second;
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
  public async Task UpdateAsync_ShouldUpdateEntityAndRemoveLinks_WhenNavigationIsOneToOneInterAggregateAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    Person first = Person.One();
    Person second = Person.One();
    string? oldSecondName = second.Name;

    var sut = _fixture.CreateRepository<Person, Guid>(dependencies, seeder =>
    {
      seeder.Seed(first);
      seeder.Seed(second);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    first.Name = "New first name";
    second.Name = "New second name";
    trackingContext.SetTrackingState(second, TrackingState.Removed);

    // Act
    await sut.UpdateAsync(first);

    // Assert
    Person? fromDb = _fixture.DbContext.Set<Person>()
      .Include(x => x.Partner)
      .FirstOrDefault(x => x.Id == first.Id);
    Person? partner = _fixture.DbContext.Set<Person>().FirstOrDefault(person => person.Id == second.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(first.Name);
    fromDb.Partner.Should().BeNull();
    partner.Should().NotBeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldThrow_WhenNavigationIsOneToOneInterAggregateAndNavigationIsModified(RepositoryDependencies dependencies)
  {
    // Arrange
    Person first = Person.One();
    Person second = Person.One();
    first.Partner = second;

    var sut = _fixture.CreateRepository<Person, Guid>(dependencies, seeder =>
    {
      seeder.Seed(first);
      seeder.Seed(second);
    }, seedImplementation: RepositoryImplementation.EFCore);

    ITrackingContext trackingContext = _fixture.TrackingContext;

    trackingContext.SetTrackingState(second, TrackingState.Modified);

    // Act
    Func<Task> act = () => sut.UpdateAsync(first);

    // Assert
    await act.Should().ThrowAsync<InvalidTrackingStateException>();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateAsync_ShouldUpdateEntityAndAddOrModifyOrDeleteChildren_WhenNavigationIsOneToManyIntraAggregate(RepositoryDependencies dependencies)
  {
    // Arrange
    Cart cart = Cart.One();
    List<CartItem> items = CartItem.Many(2, cart.Id);
    cart.Items = new(items);

    var sut = _fixture.CreateRepository<Cart, Guid>(dependencies, seeder =>
    {
      seeder.Seed(cart);
      seeder.Seed(items);
    });
    ITrackingContext trackingContext = _fixture.TrackingContext;

    cart.Name = "New cart name";
    items[1].Name = "New item name";
    CartItem item2 = CartItem.One(cart.Id);
    cart.Items.Add(item2);
    trackingContext.SetTrackingState(items[0], TrackingState.Removed);
    trackingContext.SetTrackingState(items[1], TrackingState.Modified);
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
      .And.ContainSingle(item => item.Index == items[1].Index && item.Name == items[1].Name)
      .And.ContainSingle(item => item.Index == item2.Index && item.Name == item2.Name);
  }
}
