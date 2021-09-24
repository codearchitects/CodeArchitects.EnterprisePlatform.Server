using CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTOAssociated;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore
{
  public class OTOAssociatedTests : EFCoreIntegrationTest<OTOAssociatedDbContext>
  {
    private readonly Repository<Primary, Guid> _sut;

    public OTOAssociatedTests(ITestOutputHelper output)
      : base(output)
    {
      _sut = new Repository<Primary, Guid>(Context, Context.Primaries);
    }

    protected override OTOAssociatedDbContext CreateContext(DbContextOptions<OTOAssociatedDbContext> options) => new OTOAssociatedDbContext(options);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Add_ShouldAddPrimary_WhenSecondaryIsNull(bool hasId)
    {
      // Arrange
      Primary primary0 = new Primary
      {
        Id = hasId ? Guid.NewGuid() : Guid.Empty,
        Name = "Primary0",
        Secondary = null
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
    public void Add_ShouldAddPrimaryAndCreateRelationship_WhenSecondaryIsNotNull(bool hasId)
    {
      // Arrange
      Secondary secondary0 = new Secondary
      {
        Id = hasId ? Guid.NewGuid() : Guid.Empty,
        Name = "Secondary0"
      };
      Context.Add(secondary0);
      Context.SaveChangesAndClearTracking();
      Guid secondary0Id = secondary0.Id;

      Secondary secondary0Disconnected = new Secondary
      {
        Id = secondary0Id
      };
      Primary primary0 = new Primary
      {
        Id = hasId ? Guid.NewGuid() : Guid.Empty,
        Name = "Primary0",
        Secondary = secondary0Disconnected
      };

      // Act
      _sut.Add(primary0);
      Context.SaveChangesAndClearTracking();
      Guid primary0Id = primary0.Id;

      // Assert
      Context.Primaries.Include(x => x.Secondary).Should().HaveCount(1)
        .And.ContainSingle(x => x.Id == primary0Id)
        .Which.Secondary!.Id.Should().Be(secondary0Id);
    }

    [Fact]
    public void Update_ShouldUpdatePrimary_WhenSecondaryIsNull()
    {
      // Arrange
      Secondary secondary0 = new Secondary
      {
        Name = "Secondary0"
      };
      Primary primary0 = new Primary
      {
        Name = "Primary0",
        Secondary = secondary0
      };
      Context.Add(primary0);
      Context.SaveChangesAndClearTracking();
      Guid primary0Id = primary0.Id;
      Guid secondary0Id = secondary0.Id;

      Primary disconnectedPrimary0 = new Primary
      {
        Id = primary0Id,
        Name = "Updated Primary0",
        Secondary = null
      };

      // Act
      _sut.Update(disconnectedPrimary0);
      Context.SaveChangesAndClearTracking();

      // Assert
      Context.Primaries.Include(x => x.Secondary).Should().HaveCount(1)
        .And.ContainSingle(x => x.Id == primary0Id && x.Name == "Updated Primary0")
        .Which.Secondary!.Id.Should().Be(secondary0Id);
    }

    [Fact]
    public void Update_ShouldUpdatePrimaryAndIgnoreSecondary_WhenSecondaryHasExistingId()
    {
      // Arrange
      Secondary secondary0 = new Secondary
      {
        Name = "Secondary0"
      };
      Primary primary0 = new Primary
      {
        Name = "Primary0",
        Secondary = secondary0
      };
      Context.Add(primary0);
      Context.SaveChangesAndClearTracking();
      Guid primary0Id = primary0.Id;
      Guid secondary0Id = secondary0.Id;

      Secondary disconnectedSecondary0 = new Secondary
      {
        Id = secondary0Id,
        Name = "Updated Secondary0"
      };
      Primary disconnectedPrimary0 = new Primary
      {
        Id = primary0Id,
        Name = "Updated Primary0",
        Secondary = disconnectedSecondary0
      };

      // Act
      _sut.Update(disconnectedPrimary0);
      Context.SaveChangesAndClearTracking();

      // Assert
      Context.Primaries.Include(x => x.Secondary).Should().HaveCount(1)
        .And.ContainSingle(x => x.Id == primary0Id && x.Name == "Updated Primary0")
        .Which.Secondary!.Should().Match<Secondary>(x => x.Id == secondary0Id && x.Name == "Secondary0");
    }

    [Fact]
    public void Delete_ShouldDeletePrimaryOnly()
    {
      // Arrange
      Secondary secondary0 = new Secondary
      {
        Name = "Secondary0"
      };
      Primary primary0 = new Primary
      {
        Name = "Primary0",
        Secondary = secondary0
      };
      Context.Add(primary0);
      Context.SaveChangesAndClearTracking();
      Guid primary0Id = primary0.Id;
      Guid secondary0Id = secondary0.Id;

      Primary disconnectedPrimary0 = new Primary
      {
        Id = primary0Id
      };

      // Act
      _sut.Delete(disconnectedPrimary0);
      Context.SaveChangesAndClearTracking();

      // Assert
      Context.Primaries.Should().BeEmpty();
      Context.Secondaries.Should().HaveCount(1).And.ContainSingle(x => x.Id == secondary0Id);
    }
  }
}
