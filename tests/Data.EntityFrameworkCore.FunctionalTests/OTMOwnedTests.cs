using CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTMOwned;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class OTMOwnedTests : EFCoreIntegrationTest<OTMOwnedDbContext>
{
  private readonly Repository<Primary, Guid> _sut;

  public OTMOwnedTests(ITestOutputHelper output)
    : base(output)
  {
    _sut = new Repository<Primary, Guid>(Context, Context.Primaries);
  }

  protected override OTMOwnedDbContext CreateContext(DbContextOptions<OTMOwnedDbContext> options) => new OTMOwnedDbContext(options);

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
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;

    // Assert
    Context.Primaries.Should().HaveCount(1).And.ContainSingle(x => x.Id == primary0Id);
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void Add_ShouldAddPrimaryAndSecondaries_WhenCollectionIsNotNullOrEmpty(bool hasId)
  {
    // Arrange
    Secondary secondary0 = new Secondary
    {
      Id = hasId ? Guid.NewGuid() : Guid.Empty,
      Name = "Secondary0"
    };
    Secondary secondary1 = new Secondary
    {
      Id = hasId ? Guid.NewGuid() : Guid.Empty,
      Name = "Secondary1"
    };
    Primary primary0 = new Primary
    {
      Id = hasId ? Guid.NewGuid() : Guid.Empty,
      Name = "Primary0",
      Secondaries = new Secondary[] { secondary0, secondary1 }
    };

    // Act
    _sut.Add(primary0);
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;
    Guid secondary0Id = secondary0.Id;
    Guid secondary1Id = secondary1.Id;

    // Assert
    Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(1)
      .And.ContainSingle(x => x.Id == primary0Id)
      .Which.Secondaries.Should().HaveCount(2)
        .And.ContainSingle(x => x.Id == secondary0Id)
        .And.ContainSingle(x => x.Id == secondary1Id);
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void Update_ShouldUpdatePrimary_WhenCollectionIsNullOrEmpty(bool empty)
  {
    // Arrange
    Secondary secondary0 = new Secondary
    {
      Name = "Secondary0"
    };
    Secondary secondary1 = new Secondary
    {
      Name = "Secondary1"
    };
    Primary primary0 = new Primary
    {
      Name = "Primary0",
      Secondaries = new Secondary[] { secondary0, secondary1 }
    };
    Context.Add(primary0);
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;
    Guid secondary0Id = secondary0.Id;
    Guid secondary1Id = secondary1.Id;

    Primary disconnectedPrimary0 = new Primary
    {
      Id = primary0Id,
      Name = "Updated Primary0",
      Secondaries = empty ? Array.Empty<Secondary>() : null
    };

    // Act
    _sut.Update(disconnectedPrimary0);
    Context.SaveChangesAndClearTracking();

    // Assert
    Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(1)
      .And.ContainSingle(x => x.Id == primary0Id && x.Name == "Updated Primary0")
      .Which.Secondaries.Should().HaveCount(2)
        .And.ContainSingle(x => x.Id == secondary0Id && x.Name == "Secondary0")
        .And.ContainSingle(x => x.Id == secondary1Id && x.Name == "Secondary1");
  }

  [Fact]
  public void Update_ShouldUpdatePrimaryAndSecondary_WhenCollectionContainsSecondaryWithExistingId()
  {
    // Arrange
    Secondary secondary0 = new Secondary
    {
      Name = "Secondary0"
    };
    Secondary secondary1 = new Secondary
    {
      Name = "Secondary1"
    };
    Primary primary0 = new Primary
    {
      Name = "Primary0",
      Secondaries = new Secondary[] { secondary0, secondary1 }
    };
    Context.Add(primary0);
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;
    Guid secondary0Id = secondary0.Id;
    Guid secondary1Id = secondary1.Id;

    Secondary disconnectedSecondary0 = new Secondary
    {
      Id = secondary0Id,
      Name = "Updated Secondary0"
    };
    Primary disconnectedPrimary0 = new Primary
    {
      Id = primary0Id,
      Name = "Updated Primary0",
      Secondaries = new Secondary[] { disconnectedSecondary0 }
    };

    // Act
    _sut.Update(disconnectedPrimary0);
    Context.SaveChangesAndClearTracking();

    // Assert
    Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(1)
      .And.ContainSingle(x => x.Id == primary0Id && x.Name == "Updated Primary0")
      .Which.Secondaries.Should().HaveCount(2)
        .And.ContainSingle(x => x.Id == secondary0Id && x.Name == "Updated Secondary0")
        .And.ContainSingle(x => x.Id == secondary1Id && x.Name == "Secondary1");
  }

  [Fact]
  public void Update_ShouldUpdatePrimaryAndAddSecondary_WhenCollectionContainsSecondaryWithDefaultId()
  {
    // Arrange
    Secondary secondary0 = new Secondary
    {
      Name = "Secondary0"
    };
    Secondary secondary1 = new Secondary
    {
      Name = "Secondary1"
    };
    Primary primary0 = new Primary
    {
      Name = "Primary0",
      Secondaries = new Secondary[] { secondary0, secondary1 }
    };
    Context.Add(primary0);
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;
    Guid secondary0Id = secondary0.Id;
    Guid secondary1Id = secondary1.Id;

    Secondary disconnectedSecondary2 = new Secondary
    {
      Name = "Secondary2"
    };
    Primary disconnectedPrimary0 = new Primary
    {
      Id = primary0Id,
      Name = "Updated Primary0",
      Secondaries = new Secondary[] { disconnectedSecondary2 }
    };

    // Act
    _sut.Update(disconnectedPrimary0);
    Context.SaveChangesAndClearTracking();
    Guid secondary2Id = disconnectedSecondary2.Id;

    // Assert
    Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(1)
      .And.ContainSingle(x => x.Id == primary0Id && x.Name == "Updated Primary0")
      .Which.Secondaries.Should().HaveCount(3)
        .And.ContainSingle(x => x.Id == secondary0Id && x.Name == "Secondary0")
        .And.ContainSingle(x => x.Id == secondary1Id && x.Name == "Secondary1")
        .And.ContainSingle(x => x.Id == secondary2Id && x.Name == "Secondary2");
  }

  [Fact]
  public void Delete_ShouldDeletePrimaryAndSecondary()
  {
    // Arrange
    Secondary secondary0 = new Secondary
    {
      Name = "Secondary0"
    };
    Secondary secondary1 = new Secondary
    {
      Name = "Secondary1"
    };
    Primary primary0 = new Primary
    {
      Name = "Primary0",
      Secondaries = new Secondary[] { secondary0, secondary1 }
    };
    Context.Add(primary0);
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;

    Primary disconnectedPrimary0 = new Primary
    {
      Id = primary0Id
    };

    // Act
    _sut.Delete(disconnectedPrimary0);
    Context.SaveChangesAndClearTracking();

    // Assert
    Context.Primaries.Should().BeEmpty();
    Context.Secondaries.Should().BeEmpty();
  }
}
