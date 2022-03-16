using CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTMAssociated;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class OTMAssociatedTests : EFCoreIntegrationTest<OTMAssociatedDbContext>
{
  private readonly Repository<Primary, Guid> _sut;

  public OTMAssociatedTests(ITestOutputHelper output)
    : base(output)
  {
    _sut = new Repository<Primary, Guid>(Context, Context.Primaries);
  }

  protected override OTMAssociatedDbContext CreateContext(DbContextOptions<OTMAssociatedDbContext> options) => new OTMAssociatedDbContext(options);

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void Insert_ShouldAddPrimary_WhenCollectionIsNullOrEmpty(bool empty)
  {
    // Arrange
    Primary primary0 = new Primary
    {
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

  [Fact]
  public void Insert_ShouldAddPrimaryAndCreateRelationship_WhenCollectionIsNotNullOrEmpty()
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
    Context.AddRange(secondary0, secondary1);
    Context.SaveChangesAndClearTracking();
    Guid secondary0Id = secondary0.Id;
    Guid secondary1Id = secondary1.Id;

    Secondary secondary0Disconnected = new Secondary
    {
      Id = secondary0Id
    };
    Primary primary0 = new Primary
    {
      Name = "Primary0",
      Secondaries = new Secondary[] { secondary0Disconnected }
    };

    // Act
    _sut.Add(primary0);
    EntityState primary0State = Context.Entry(primary0).State;
    EntityState secondary0State = Context.Entry(secondary0Disconnected).State;
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;

    // Assert
    primary0State.Should().Be(EntityState.Added);
    Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(1)
      .And.ContainSingle(x => x.Id == primary0Id)
      .Which.Secondaries.Should().HaveCount(1)
        .And.ContainSingle(x => x.Id == secondary0Id);
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
    Context.AddRange(secondary0, secondary1);
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
    EntityState primary0State = Context.Entry(disconnectedPrimary0).State;
    Context.SaveChangesAndClearTracking();

    // Assert
    primary0State.Should().Be(EntityState.Modified);
    Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(1)
      .And.ContainSingle(x => x.Id == primary0Id && x.Name == "Updated Primary0")
      .Which.Secondaries.Should().HaveCount(2)
        .And.ContainSingle(x => x.Id == secondary0Id && x.Name == "Secondary0")
        .And.ContainSingle(x => x.Id == secondary1Id && x.Name == "Secondary1");
  }

  [Fact]
  public void Update_ShouldUpdatePrimaryAndCreateRelationship_WhenCollectionContainsSecondaryWithExistingId()
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
      Secondaries = new Secondary[] { secondary0 }
    };
    Context.AddRange(secondary0, secondary1);
    Context.Add(primary0);
    Context.SaveChangesAndClearTracking();
    Guid primary0Id = primary0.Id;
    Guid secondary0Id = secondary0.Id;
    Guid secondary1Id = secondary1.Id;

    Secondary disconnectedSecondary1 = new Secondary
    {
      Id = secondary1Id
    };
    Primary disconnectedPrimary0 = new Primary
    {
      Id = primary0Id,
      Name = "Updated Primary0",
      Secondaries = new Secondary[] { disconnectedSecondary1 }
    };

    // Act
    _sut.Update(disconnectedPrimary0);
    EntityState primary0State = Context.Entry(disconnectedPrimary0).State;
    EntityState secondary1State = Context.Entry(disconnectedSecondary1).State;
    Context.SaveChangesAndClearTracking();

    // Assert
    primary0State.Should().Be(EntityState.Modified);
    secondary1State.Should().Be(EntityState.Modified);
    Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(1)
      .And.ContainSingle(x => x.Id == primary0Id && x.Name == "Updated Primary0")
      .Which.Secondaries.Should().HaveCount(2)
        .And.ContainSingle(x => x.Id == secondary0Id && x.Name == "Secondary0")
        .And.ContainSingle(x => x.Id == secondary1Id && x.Name == "Secondary1");
  }

  [Fact]
  public void Delete_ShouldDeletePrimaryOnly()
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
      Id = primary0Id
    };

    // Act
    _sut.Delete(disconnectedPrimary0);
    Context.SaveChangesAndClearTracking();

    // Assert
    Context.Primaries.Should().BeEmpty();
    Context.Secondaries.Should().HaveCount(2)
      .And.ContainSingle(x => x.Id == secondary0Id)
      .And.ContainSingle(x => x.Id == secondary1Id);
  }
}
