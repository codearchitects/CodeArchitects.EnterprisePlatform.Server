using CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTMDirect;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class MTMDirectTests : EFCoreIntegrationTest<MTMDirectDbContext>
{
  private readonly Repository<Primary, Guid> _sut;

  public MTMDirectTests(ITestOutputHelper output)
    : base(output)
  {
    _sut = new Repository<Primary, Guid>(Context, Context.Primaries);
  }

  protected override MTMDirectDbContext CreateContext(DbContextOptions<MTMDirectDbContext> options) => new MTMDirectDbContext(options);

  [Theory]
  [InlineData(true, true)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(false, false)]
  public void Add_ShouldAddPrimary_WhenCollectionIsNullOrEmpty(bool empty, bool hasId)
  {
    // Arrange
    Primary primary0 = new Primary
    {
      Id = hasId ? Guid.NewGuid() : Guid.Empty,
      Name = "Primary0",
      Secondaries = empty ? Array.Empty<Secondary>() : null
    };

    // Act
    _sut.Add(primary0);
    EntityState primary0State = Context.Entry(primary0).State;
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;

    // Assert
    primary0State.Should().Be(EntityState.Added);
    Context.Primaries.Should().HaveCount(1).And.ContainSingle(x => x.Id == primary0Id);
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void Add_ShouldAddPrimaryAndCreateRelationship_WhenCollectionIsNotNullOrEmpty(bool hasId)
  {
    // Arrange
    Primary primary0 = new Primary
    {
      Name = "Primary0"
    };
    Secondary secondary0 = new Secondary
    {
      Name = "Secondary0",
      Primaries = new Primary[] { primary0 }
    };
    Secondary secondary1 = new Secondary
    {
      Name = "Secondary1",
      Primaries = new Primary[] { primary0 }
    };
    Context.AddRange(secondary0, secondary1);
    Context.Add(primary0);
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;
    Guid secondary0Id = secondary0.Id;
    Guid secondary1Id = secondary1.Id;

    Secondary secondary0Disconnected = new Secondary
    {
      Id = secondary0Id
    };
    Primary primary1 = new Primary
    {
      Id = hasId ? Guid.NewGuid() : Guid.Empty,
      Name = "Primary1",
      Secondaries = new Secondary[] { secondary0Disconnected }
    };

    // Act
    _sut.Add(primary1);
    Context.SaveChangesAndClearTracking();
    Guid primary1Id = primary1.Id;

    // Assert
    Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(2)
      .And.ContainSingle(x => x.Id == primary0Id)
      .And.ContainSingle(x => x.Id == primary1Id)
      .Which.Secondaries.Should().HaveCount(1)
        .And.ContainSingle(x => x.Id == secondary0Id);
  }

  [Fact]
  public void Update_ShouldUpdatePrimaryAndCreateRelationship_WhenCollectionContainsSecondaryWithExistingId()
  {
    // Arrange
    Primary primary0 = new Primary
    {
      Name = "Primary0"
    };
    Secondary secondary0 = new Secondary
    {
      Name = "Secondary0",
      Primaries = new Primary[] { primary0 }
    };
    Secondary secondary1 = new Secondary
    {
      Name = "Secondary1"
    };

    Context.AddRange(secondary0, secondary1);
    Context.Add(primary0);
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;
    Guid secondary0Id = secondary0.Id;
    Guid secondary1Id = secondary1.Id;

    Secondary secondary0Disconnected = new Secondary
    {
      Id = secondary0Id
    };
    Secondary secondary1Disconnected = new Secondary
    {
      Id = secondary1Id
    };
    Primary primary0Disconnected = new Primary
    {
      Id = primary0Id,
      Name = "Updated Primary0",
      Secondaries = new Secondary[] { secondary0Disconnected, secondary1Disconnected }
    };

    // Act
    _sut.Update(primary0Disconnected);
    Context.SaveChangesAndClearTracking();

    // Assert
    Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(1)
      .And.ContainSingle(x => x.Id == primary0Id && x.Name == "Updated Primary0")
      .Which.Secondaries.Should().HaveCount(2)
        .And.ContainSingle(x => x.Id == secondary0Id)
        .And.ContainSingle(x => x.Id == secondary1Id);
  }

  [Fact]
  public void Delete_ShouldDeletePrimaryOnly()
  {
    // Arrange
    Primary primary0 = new Primary
    {
      Name = "Primary0"
    };
    Secondary secondary0 = new Secondary
    {
      Name = "Secondary0",
      Primaries = new Primary[] { primary0 }
    };
    Secondary secondary1 = new Secondary
    {
      Name = "Secondary1",
      Primaries = new Primary[] { primary0 }
    };

    Context.AddRange(secondary0, secondary1);
    Context.Add(primary0);
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;
    Guid secondary0Id = secondary0.Id;
    Guid secondary1Id = secondary1.Id;

    Primary primary0Disconnected = new Primary
    {
      Id = primary0Id
    };

    // Act
    _sut.Delete(primary0Disconnected);
    Context.SaveChangesAndClearTracking();

    // Assert
    Context.Primaries.Should().HaveCount(0);
    Context.Secondaries.Should().HaveCount(2)
        .And.ContainSingle(x => x.Id == secondary0Id)
        .And.ContainSingle(x => x.Id == secondary1Id);
  }
}
