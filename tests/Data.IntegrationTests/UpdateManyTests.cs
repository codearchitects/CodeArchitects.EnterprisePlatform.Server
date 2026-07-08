using CodeArchitects.Platform.Data.Fixtures;
using CodeArchitects.Platform.Data.Fixtures.Model;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data;

[Collection(TestCollection.Name)]
public class UpdateManyTests : TestBase
{
  public UpdateManyTests(TestFixture fixture, ITestOutputHelper output)
    : base(fixture, output)
  {
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityOnly_WhenNavigationIsManyToMany(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Category> categories = Category.Many(2);
    List<Typology> typologies0 = Typology.Many(2);
    List<Typology> typologies1 = Typology.Many(2);
    categories[0].Typologies = new(typologies0);
    categories[1].Typologies = new(typologies1);
    string? oldTypologyName = typologies0[0].Name;


    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(seeder =>
    {
      seeder.Seed(typologies0);
      seeder.Seed(typologies1);
      seeder.Seed(categories);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Category, Guid>();

    categories[0].Name = "New category0 name";
    categories[1].Name = "New category1 name";
    typologies0[0].Name = "New typology name 0";

    // Act
    await sut.UpdateManyAsync(categories);

    // Assert
    IQueryable<Category> queryable = scope.DbContext.Set<Category>().Include(x => x.Typologies);
    Category?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == categories[0].Id),
      queryable.FirstOrDefault(x => x.Id == categories[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(categories[0].Name);
    fromDb[0]!.Typologies.Should().HaveCount(2)
      .And.ContainSingle(typology => typology.Id == typologies0[0].Id && typology.Name == oldTypologyName)
      .And.ContainSingle(typology => typology.Id == typologies0[1].Id && typology.Name == typologies0[1].Name);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(categories[1].Name);
    fromDb[1]!.Typologies.Should().HaveCount(2)
      .And.ContainSingle(typology => typology.Id == typologies1[0].Id && typology.Name == typologies1[0].Name)
      .And.ContainSingle(typology => typology.Id == typologies1[1].Id && typology.Name == typologies1[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityAndAddLinks_WhenNavigationIsManyToManyAndNavigationIsAdded(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Category> categories = Category.Many(2);
    List<Typology> typologies0 = Typology.Many(2);
    List<Typology> typologies1 = Typology.Many(2);
    categories[0].Typologies = new() { typologies0[0] };
    categories[1].Typologies = new(typologies1);
    string? oldTypologyName = typologies0[0].Name;

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(typologies0);
      seeder.Seed(typologies1);
      seeder.Seed(categories);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Category, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    categories[0].Name = "New category0 name";
    categories[1].Name = "New category1 name";
    typologies0[0].Name = "New typology name";
    categories[0].Typologies!.Add(typologies0[1]);
    trackingContext.SetTrackingState(typologies0[0], TrackingState.Unchanged);
    trackingContext.SetTrackingState(typologies0[1], TrackingState.Added);

    // Act
    await sut.UpdateManyAsync(categories);

    // Assert
    IQueryable<Category> queryable = scope.DbContext.Set<Category>().Include(x => x.Typologies);
    Category?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == categories[0].Id),
      queryable.FirstOrDefault(x => x.Id == categories[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(categories[0].Name);
    fromDb[0]!.Typologies.Should().HaveCount(2)
      .And.ContainSingle(typology => typology.Id == typologies0[0].Id && typology.Name == oldTypologyName)
      .And.ContainSingle(typology => typology.Id == typologies0[1].Id && typology.Name == typologies0[1].Name);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(categories[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityAndRemoveLinks_WhenNavigationIsManyToManyAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Category> categories = Category.Many(2);
    List<Typology> typologies0 = Typology.Many(2);
    List<Typology> typologies1 = Typology.Many(2);
    categories[0].Typologies = new(typologies0);
    categories[1].Typologies = new(typologies1);
    string? oldTypologyName = typologies0[0].Name;

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(typologies0);
      seeder.Seed(typologies1);
      seeder.Seed(categories);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Category, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    categories[0].Name = "New category0 name";
    categories[1].Name = "New category1 name";
    typologies0[0].Name = "New typology name";
    trackingContext.SetTrackingState(typologies0[0], TrackingState.Unchanged);
    trackingContext.SetTrackingState(typologies0[1], TrackingState.Removed);

    // Act
    await sut.UpdateManyAsync(categories);

    // Assert
    IQueryable<Category> queryable = scope.DbContext.Set<Category>().Include(x => x.Typologies);
    Category?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == categories[0].Id),
      queryable.FirstOrDefault(x => x.Id == categories[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(categories[0].Name);
    fromDb[0]!.Typologies.Should().HaveCount(1)
      .And.ContainSingle(typology => typology.Id == typologies0[0].Id && typology.Name == oldTypologyName);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(categories[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldThrow_WhenNavigationIsManyToManyAndNavigationIsModified(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Category> categories = Category.Many(2);
    List<Typology> typologies0 = Typology.Many(2);
    List<Typology> typologies1 = Typology.Many(2);
    categories[0].Typologies = new(typologies0);
    categories[1].Typologies = new(typologies1);

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(typologies0);
      seeder.Seed(typologies1);
      seeder.Seed(categories);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Category, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    trackingContext.SetTrackingState(typologies0[0], TrackingState.Modified);
    trackingContext.SetTrackingState(typologies0[1], TrackingState.Modified);

    // Act
    Func<Task> act = () => sut.UpdateManyAsync(categories);

    // Assert
    await act.Should().ThrowAsync<InvalidTrackingStateException>();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityOnly_WhenNavigationIsOneToManyIntraAggregate(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Customer> customers = Customer.Many(2);
    List<Cart> carts0 = Cart.Many(2);
    List<Cart> carts1 = Cart.Many(2);
    customers[0].Carts = new(carts0);
    customers[1].Carts = new(carts1);
    string? oldCartName = carts0[0].Name;

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(carts0);
      seeder.Seed(carts1);
      seeder.Seed(customers);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Customer, Guid>();

    customers[0].Name = "New customer0 name";
    customers[1].Name = "New customer1 name";
    carts0[0].Name = "New cart name";

    // Act
    await sut.UpdateManyAsync(customers);

    // Assert
    IQueryable<Customer> queryable = scope.DbContext.Set<Customer>().Include(x => x.Carts);
    Customer?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == customers[0].Id),
      queryable.FirstOrDefault(x => x.Id == customers[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(customers[0].Name);
    fromDb[0]!.Carts.Should().HaveCount(2)
      .And.ContainSingle(cart => cart.Id == carts0[0].Id && cart.Name == oldCartName)
      .And.ContainSingle(cart => cart.Id == carts0[1].Id && cart.Name == carts0[1].Name);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(customers[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityAndAddLinks_WhenNavigationIsOneToManyInterAggregateAndNavigationIsAdded(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Customer> customers = Customer.Many(2);
    List<Cart> carts0 = Cart.Many(2);
    List<Cart> carts1 = Cart.Many(2);
    customers[0].Carts = new() { carts0[0] };
    customers[1].Carts = new(carts1);
    string? oldCartName = carts0[0].Name;

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(carts0);
      seeder.Seed(carts1);
      seeder.Seed(customers);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Customer, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    customers[0].Name = "New customer0 name";
    customers[1].Name = "New customer1 name";
    carts0[0].Name = "New cart name";
    customers[0].Carts!.Add(carts0[1]);
    trackingContext.SetTrackingState(carts0[0], TrackingState.Unchanged);
    trackingContext.SetTrackingState(carts0[1], TrackingState.Added);

    // Act
    await sut.UpdateManyAsync(customers);

    // Assert
    IQueryable<Customer> queryable = scope.DbContext.Set<Customer>().Include(x => x.Carts);
    Customer?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == customers[0].Id),
      queryable.FirstOrDefault(x => x.Id == customers[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(customers[0].Name);
    fromDb[0]!.Carts.Should().HaveCount(2)
      .And.ContainSingle(cart => cart.Id == carts0[0].Id && cart.Name == oldCartName)
      .And.ContainSingle(cart => cart.Id == carts0[1].Id && cart.Name == carts0[1].Name);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(customers[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityAndRemoveLinks_WhenNavigationIsOneToManyInterAggregateAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Customer> customers = Customer.Many(2);
    List<Cart> carts0 = Cart.Many(2);
    List<Cart> carts1 = Cart.Many(2);
    customers[0].Carts = new(carts0);
    customers[1].Carts = new(carts1);
    string? oldCartName = carts0[0].Name;

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(carts0);
      seeder.Seed(carts1);
      seeder.Seed(customers);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Customer, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    customers[0].Name = "New customer0 name";
    customers[1].Name = "New customer1 name";
    carts0[0].Name = "New cart name";
    trackingContext.SetTrackingState(carts0[0], TrackingState.Unchanged);
    trackingContext.SetTrackingState(carts0[1], TrackingState.Removed);

    // Act
    await sut.UpdateManyAsync(customers);

    // Assert
    IQueryable<Customer> queryable = scope.DbContext.Set<Customer>().Include(x => x.Carts);
    Customer?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == customers[0].Id),
      queryable.FirstOrDefault(x => x.Id == customers[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(customers[0].Name);
    fromDb[0]!.Carts.Should().HaveCount(1)
      .And.ContainSingle(cart => cart.Id == carts0[0].Id && cart.Name == oldCartName);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(customers[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityAndRemoveOrAddLinksOfAggregateEntity_WhenNavigationIsManyToManyAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Product> products = Product.Many(3);
    List<Cart> carts = Cart.Many(2);
    CartItem item0 = CartItem.One(carts[0].Id);
    CartItem item1 = CartItem.One(carts[1].Id);
    item0.Products = new() { products[0], products[1] };
    carts[0].Items = new() { item0 };
    carts[1].Items = new() { item1 };

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(carts);
      seeder.Seed(item0);
      seeder.Seed(item1);
      seeder.Seed(products);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Cart, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    carts[0].Name = "New cart0 name";
    carts[1].Name = "New cart1 name";
    item0.Name = "New item name";
    item0.Products = new() { products[0], products[2] };
    trackingContext.SetTrackingState(item0, TrackingState.Modified);
    trackingContext.SetTrackingState(products[0], TrackingState.Removed);
    trackingContext.SetTrackingState(products[2], TrackingState.Added);

    // Act
    await sut.UpdateManyAsync(carts);

    // Assert
    IQueryable<Cart> queryable = scope.DbContext.Set<Cart>()
      .Include(x => x.Items!)
      .ThenInclude(x => x.Products);
    Cart?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == carts[0].Id),
      queryable.FirstOrDefault(x => x.Id == carts[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(carts[0].Name);
    fromDb[0]!.Items.Should().HaveCount(1);
    fromDb[0]!.Items![0].Products.Should().HaveCount(2)
      .And.ContainSingle(product => product.Id == products[1].Id)
      .And.ContainSingle(product => product.Id == products[2].Id);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(carts[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldThrow_WhenNavigationIsOneToManyInterAggregateAndNavigationIsModified(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Customer> customers = Customer.Many(2);
    List<Cart> carts0 = Cart.Many(2);
    List<Cart> carts1 = Cart.Many(2);
    customers[0].Carts = new(carts0);
    customers[1].Carts = new(carts1);

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(carts0);
      seeder.Seed(carts1);
      seeder.Seed(customers);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Customer, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    trackingContext.SetTrackingState(carts0[0], TrackingState.Modified);
    trackingContext.SetTrackingState(carts0[1], TrackingState.Modified);

    // Act
    Func<Task> act = () => sut.UpdateManyAsync(customers);

    // Assert
    await act.Should().ThrowAsync<InvalidTrackingStateException>();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityOnly_WhenNavigationIsOneToOneInterAggregate(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Person> firsts = Person.Many(2);
    List<Person> seconds = Person.Many(2);
    firsts[0].Partner = seconds[0];
    string? oldSecondName = seconds[0].Name;

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(firsts);
      seeder.Seed(seconds);
    }, seedImplementation: DataImplementation.EFCore);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Person, Guid>();

    firsts[0].Name = "New first0 name";
    firsts[1].Name = "New first1 name";
    seconds[0].Name = "New second name";

    // Act
    await sut.UpdateManyAsync(firsts);

    // Assert
    IQueryable<Person> queryable = scope.DbContext.Set<Person>().Include(x => x.Partner);
    Person?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == firsts[0].Id),
      queryable.FirstOrDefault(x => x.Id == firsts[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(firsts[0].Name);
    fromDb[0]!.Partner!.Name.Should().Be(oldSecondName);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(firsts[1].Name);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityAndAddLinks_WhenNavigationIsOneToOneInterAggregateAndNavigationIsAdded(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Person> firsts = Person.Many(2);
    List<Person> seconds = Person.Many(2);
    string? oldSecondName0 = seconds[0].Name;
    string? oldSecondName1 = seconds[1].Name;

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(firsts);
      seeder.Seed(seconds);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Person, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    firsts[0].Name = "New first0 name";
    firsts[1].Name = "New first1 name";
    seconds[0].Name = "New second0 name";
    seconds[0].Name = "New second1 name";
    firsts[0].Partner = seconds[0];
    firsts[1].Partner = seconds[1];
    trackingContext.SetTrackingState(seconds[0], TrackingState.Added);
    trackingContext.SetTrackingState(seconds[1], TrackingState.Added);

    // Act
    await sut.UpdateManyAsync(firsts);

    // Assert
    IQueryable<Person> queryable = scope.DbContext.Set<Person>().Include(x => x.Partner);
    Person?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == firsts[0].Id),
      queryable.FirstOrDefault(x => x.Id == firsts[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(firsts[0].Name);
    fromDb[0]!.Partner.Should().NotBeNull();
    fromDb[0]!.Partner!.Id.Should().Be(seconds[0].Id);
    fromDb[0]!.Partner!.Name.Should().Be(oldSecondName0);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(firsts[1].Name);
    fromDb[1]!.Partner.Should().NotBeNull();
    fromDb[1]!.Partner!.Id.Should().Be(seconds[1].Id);
    fromDb[1]!.Partner!.Name.Should().Be(oldSecondName1);
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityAndRemoveLinks_WhenNavigationIsOneToOneInterAggregateAndNavigationIsRemoved(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Person> firsts = Person.Many(2);
    List<Person> seconds = Person.Many(2);
    string? oldSecondName0 = seconds[0].Name;
    string? oldSecondName1 = seconds[1].Name;

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(firsts);
      seeder.Seed(seconds);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Person, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    firsts[0].Name = "New first0 name";
    seconds[0].Name = "New second name";
    firsts[1].Name = "New first1 name";
    seconds[1].Name = "New second name";
    trackingContext.SetTrackingState(seconds[0], TrackingState.Removed);
    trackingContext.SetTrackingState(seconds[1], TrackingState.Removed);

    // Act
    await sut.UpdateManyAsync(firsts);

    // Assert
    IQueryable<Person> queryable = scope.DbContext.Set<Person>().Include(x => x.Partner);
    Person?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == firsts[0].Id),
      queryable.FirstOrDefault(x => x.Id == firsts[1].Id)
    };
    Person? partner0 = scope.DbContext.Set<Person>().FirstOrDefault(person => person.Id == seconds[0].Id);
    Person? partner1 = scope.DbContext.Set<Person>().FirstOrDefault(person => person.Id == seconds[1].Id);

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(firsts[0].Name);
    fromDb[0]!.Partner.Should().BeNull();
    partner0.Should().NotBeNull();
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(firsts[1].Name);
    fromDb[1]!.Partner.Should().BeNull();
    partner1.Should().NotBeNull();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldThrow_WhenNavigationIsOneToOneInterAggregateAndNavigationIsModified(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Person> firsts = Person.Many(2);
    List<Person> seconds = Person.Many(2);
    firsts[0].Partner = seconds[0];
    firsts[1].Partner = seconds[1];

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(firsts);
      seeder.Seed(seconds);
    }, seedImplementation: DataImplementation.EFCore);
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Person, Guid>();

    ITrackingContext trackingContext = scope.TrackingContext;

    trackingContext.SetTrackingState(seconds[0], TrackingState.Modified);
    trackingContext.SetTrackingState(seconds[1], TrackingState.Modified);

    // Act
    Func<Task> act = () => sut.UpdateManyAsync(firsts);

    // Assert
    await act.Should().ThrowAsync<InvalidTrackingStateException>();
  }

  [Theory, RepositoryDependenciesData]
  public async Task UpdateManyAsync_ShouldUpdateEntityAndAddOrModifyOrDeleteChildren_WhenNavigationIsOneToManyIntraAggregate(RepositoryDependencies dependencies)
  {
    // Arrange
    List<Cart> carts = Cart.Many(2);
    List<CartItem> items0 = CartItem.Many(2, carts[0].Id);
    List<CartItem> items1 = CartItem.Many(2, carts[1].Id);
    carts[0].Items = new(items0);
    carts[1].Items = new(items1);

    
    using var dbFixture = _fixture.CreateDbFixture(dependencies);dbFixture.Seed(seeder =>
    {
      seeder.Seed(carts);
      seeder.Seed(items0);
      seeder.Seed(items1);
    });
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Cart, Guid>();
    ITrackingContext trackingContext = scope.TrackingContext;

    carts[0].Name = "New cart0 name";
    carts[1].Name = "New cart1 name";
    items0[1].Name = "New item name";
    CartItem item2 = CartItem.One(carts[0].Id);
    carts[0].Items!.Add(item2);
    trackingContext.SetTrackingState(items0[0], TrackingState.Removed);
    trackingContext.SetTrackingState(items0[1], TrackingState.Modified);
    trackingContext.SetTrackingState(item2, TrackingState.Added);

    // Act
    await sut.UpdateManyAsync(carts);

    // Assert
    IQueryable<Cart> queryable = scope.DbContext.Set<Cart>().Include(x => x.Items!);
    Cart?[] fromDb = new[]
    {
      queryable.FirstOrDefault(x => x.Id == carts[0].Id),
      queryable.FirstOrDefault(x => x.Id == carts[1].Id)
    };

    fromDb[0].Should().NotBeNull();
    fromDb[0]!.Name.Should().Be(carts[0].Name);
    fromDb[0]!.Items.Should().HaveCount(2)
      .And.ContainSingle(item => item.Index == items0[1].Index && item.Name == items0[1].Name)
      .And.ContainSingle(item => item.Index == item2.Index && item.Name == item2.Name);
    fromDb[1].Should().NotBeNull();
    fromDb[1]!.Name.Should().Be(carts[1].Name);
  }
}
