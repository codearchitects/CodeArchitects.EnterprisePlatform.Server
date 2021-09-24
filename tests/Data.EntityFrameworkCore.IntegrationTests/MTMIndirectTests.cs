using CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTMIndirect;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore
{
  public class MTMIndirectTests : EFCoreIntegrationTest<MTMIndirectDbContext>
  {
    private readonly Repository<Primary, Guid> _sut;

    public MTMIndirectTests(ITestOutputHelper output)
      : base(output)
    {
      _sut = new Repository<Primary, Guid>(Context, Context.Primaries);
    }

    protected override MTMIndirectDbContext CreateContext(DbContextOptions<MTMIndirectDbContext> options) => new MTMIndirectDbContext(options);

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
        Secondaries = empty ? Array.Empty<PrimarySecondary>() : null
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
        Name = "Primary0",
        Secondaries = new List<PrimarySecondary>()
      };
      Secondary secondary0 = new Secondary
      {
        Name = "Secondary0",
        Primaries = new List<PrimarySecondary>()
      };
      Secondary secondary1 = new Secondary
      {
        Name = "Secondary1",
        Primaries = new List<PrimarySecondary>()
      };
      Context.AddRange(secondary0, secondary1);
      Context.Add(primary0);
      primary0.Secondaries.Add(new PrimarySecondary { PrimaryId = primary0.Id, SecondaryId = secondary0.Id });
      Context.SaveChangesAndClearTracking();
      Guid primary0Id = primary0.Id;
      Guid secondary0Id = secondary0.Id;
      Guid secondary1Id = secondary1.Id;

      Primary primary1 = new Primary
      {
        Id = hasId ? Guid.NewGuid() : Guid.Empty,
        Name = "Primary1",
        Secondaries = new[]
        {
          new PrimarySecondary { SecondaryId = secondary0Id }
        }
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
          .And.ContainSingle(x => x.SecondaryId == secondary0Id);
    }

    [Fact]
    public void Update_ShouldUpdatePrimaryAndCreateRelationship_WhenCollectionContainsSecondaryWithExistingId()
    {
      // Arrange
      Primary primary0 = new Primary
      {
        Name = "Primary0",
        Secondaries = new List<PrimarySecondary>()
      };
      Secondary secondary0 = new Secondary
      {
        Name = "Secondary0",
        Primaries = new List<PrimarySecondary>()
      };
      Secondary secondary1 = new Secondary
      {
        Name = "Secondary1",
        Primaries = new List<PrimarySecondary>()
      };

      Context.AddRange(secondary0, secondary1);
      Context.Add(primary0);
      primary0.Secondaries.Add(new PrimarySecondary { PrimaryId = primary0.Id, SecondaryId = secondary0.Id });
      Context.SaveChangesAndClearTracking();
      Guid primary0Id = primary0.Id;
      Guid secondary0Id = secondary0.Id;
      Guid secondary1Id = secondary1.Id;

      Primary primary0Disconnected = new Primary
      {
        Id = primary0Id,
        Name = "Updated Primary0",
        Secondaries = new[]
        {
          new PrimarySecondary { PrimaryId = primary0Id, SecondaryId = secondary0Id },
          new PrimarySecondary { PrimaryId = primary0Id, SecondaryId = secondary1Id },
        }
      };

      // Act
      _sut.Update(primary0Disconnected);
      Context.SaveChangesAndClearTracking();

      // Assert
      Context.Primaries.Include(x => x.Secondaries).Should().HaveCount(1)
        .And.ContainSingle(x => x.Id == primary0Id && x.Name == "Updated Primary0")
        .Which.Secondaries.Should().HaveCount(2)
          .And.ContainSingle(x => x.SecondaryId == secondary0Id)
          .And.ContainSingle(x => x.SecondaryId == secondary1Id);
    }

    [Fact]
    public void Delete_ShouldDeletePrimaryOnly()
    {
      // Arrange
      Primary primary0 = new Primary
      {
        Name = "Primary0",
        Secondaries = new List<PrimarySecondary>()
      };
      Secondary secondary0 = new Secondary
      {
        Name = "Secondary0",
        Primaries = new List<PrimarySecondary>()
      };
      Secondary secondary1 = new Secondary
      {
        Name = "Secondary1",
        Primaries = new List<PrimarySecondary>()
      };

      Context.AddRange(secondary0, secondary1);
      Context.Add(primary0);
      primary0.Secondaries.Add(new PrimarySecondary { PrimaryId = primary0.Id, SecondaryId = secondary0.Id });
      primary0.Secondaries.Add(new PrimarySecondary { PrimaryId = primary0.Id, SecondaryId = secondary1.Id });
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
}
