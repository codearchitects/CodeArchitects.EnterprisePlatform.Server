using CodeArchitects.Platform.Common.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class DataContextTests
{
  private readonly Mock<IIdentityProfile> _identityProfileMock;

  public DataContextTests()
  {
    _identityProfileMock = new Mock<IIdentityProfile>(MockBehavior.Strict);
  }

  [Fact]
  public void QueryTenantEntities_ShouldBeFilteredByTenantId()
  {
    // Arrange
    DbContextOptions<MyTenantDbContext> options = new DbContextOptionsBuilder<MyTenantDbContext>()
      .UseInMemoryDatabase("tenantdb1")
      .Options;

    Guid tenantId = Guid.NewGuid();
    _identityProfileMock
      .Setup(x => x.IsAuthenticated)
      .Returns(true);
    _identityProfileMock
      .Setup(x => x.TenantId)
      .Returns(tenantId);

    MyTenantDbContext sut = new MyTenantDbContext(_identityProfileMock.Object, options);

    sut.Add(new MyTenantEntity("1") { Descr = "descr1", TenantId = tenantId });
    sut.Add(new MyTenantEntity("2") { Descr = "descr2", TenantId = Guid.NewGuid() });
    sut.SaveChangesWithoutTenantId();
    sut.ChangeTracker.Clear();

    // Act
    List<MyTenantEntity> entities = sut.Set<MyTenantEntity>().ToList();

    // Assert
    entities.Should().HaveCount(1).And.ContainSingle(entity => entity.TenantId == tenantId);
  }

  [Fact]
  public void QueryTenantEntities_ShouldNotBeFilteredByTenantId_WhenFiltersAreIgnored()
  {
    // Arrange
    DbContextOptions<MyTenantDbContext> options = new DbContextOptionsBuilder<MyTenantDbContext>()
      .UseInMemoryDatabase("tenantdb2")
      .Options;

    Guid tenantId = Guid.NewGuid();
    _identityProfileMock
      .Setup(x => x.IsAuthenticated)
      .Returns(true);
    _identityProfileMock
      .Setup(x => x.TenantId)
      .Returns(tenantId);

    MyTenantDbContext sut = new MyTenantDbContext(_identityProfileMock.Object, options);

    sut.Add(new MyTenantEntity("1") { Descr = "descr1", TenantId = tenantId });
    sut.Add(new MyTenantEntity("2") { Descr = "descr2", TenantId = Guid.NewGuid() });
    sut.SaveChangesWithoutTenantId();
    sut.ChangeTracker.Clear();

    // Act
    List<MyTenantEntity> entities = sut.Set<MyTenantEntity>().IgnoreQueryFilters().ToList();

    // Assert
    entities.Should().HaveCount(2)
      .And.ContainSingle(entity => entity.Id == "1")
      .And.ContainSingle(entity => entity.Id == "2");
  }

  [Fact]
  public void AddTenantEntity_ShouldSetCorrectTenantId()
  {
    // Arrange
    DbContextOptions<MyTenantDbContext> options = new DbContextOptionsBuilder<MyTenantDbContext>()
      .UseInMemoryDatabase("tenantdb3")
      .Options;

    Guid tenantId = Guid.NewGuid();
    _identityProfileMock
      .Setup(x => x.IsAuthenticated)
      .Returns(true);
    _identityProfileMock
      .Setup(x => x.TenantId)
      .Returns(tenantId);

    MyTenantDbContext sut = new MyTenantDbContext(_identityProfileMock.Object, options);

    // Act
    sut.Add(new MyTenantEntity("1") { Descr = "descr1" });
    sut.SaveChanges();

    // Assert
    sut.Find<MyTenantEntity>("1").TenantId.Should().Be(tenantId);
  }

  public class MyTenantEntity : IEntity<string>, ITenantEntity
  {
    public MyTenantEntity(string id)
    {
      Id = id;
    }

    public string Id { get; set; }

    public string? Descr { get; set; }

    public Guid? TenantId { get; set; }

    object IEntity.Id => Id;
  }

  public class MyTenantDbContext : DataContext
  {
    public MyTenantDbContext(IIdentityProfile identity, DbContextOptions<MyTenantDbContext> options)
      : base(identity, options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<MyTenantEntity>();
    }
  }
}
