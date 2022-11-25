using CodeArchitects.Platform.Data.AdoNet.Fixtures;
using static CodeArchitects.Platform.Data.AdoNet.Fixtures.NavigationFixture;
using static CodeArchitects.Platform.Data.AdoNet.Fixtures.NavigationFixture.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

public class IncluderTests
{
  [Fact]
  public void Include_ShouldIncludeLeafNavigation_WhenDepth1()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include(nameof(Root.ChildA));

    // Assert
    includer.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Index == RootToChildAId);
  }

  [Fact]
  public void Include_ShouldIncludeMultipleLeafNavigations_WhenDepth1()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(nameof(Root.ChildA))
      .Include(nameof(Root.ChildB));

    // Assert
    includer.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Index == RootToChildAId)
      .And.ContainSingle(nav => nav.Index == RootToChildBId);
  }

  [Fact]
  public void Include_ShouldIncludeLeafNavigation_WhenDepth1AndIncludedMultipleTimes()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(nameof(Root.ChildA))
      .Include(nameof(Root.ChildA));

    // Assert
    includer.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Index == RootToChildAId);
  }

  [Fact]
  public void Include_ShouldIncludeNodeNavigation_WhenDepth2()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}");

    // Assert
    includer.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Index == RootToChildAId)
      .Which.As<INavigationNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Index == ChildAToChildDId);
  }

  [Fact]
  public void Include_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsLeaf()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(nameof(Root.ChildA))
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}");

    // Assert
    includer.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Index == RootToChildAId)
      .Which.As<INavigationNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Index == ChildAToChildDId);
  }

  [Fact]
  public void Include_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsNode()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}")
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildF)}");

    // Assert
    includer.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Index == RootToChildAId)
      .Which.As<INavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Index == ChildAToChildDId)
        .And.ContainSingle(nav => nav.Index == ChildAToChildFId);
  }

  [Fact]
  public void Include_ShouldIncludeAllNavigations()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}")
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildF)}")
      .Include(nameof(Root.ChildB))
      .Include(nameof(Root.ChildC));

    // Assert
    includer.Navigations.Should().HaveCount(3)
      .And.ContainSingle(nav => nav.Index == RootToChildBId)
      .And.ContainSingle(nav => nav.Index == RootToChildCId)
      .And.ContainSingle(nav => nav.Index == RootToChildAId)
      .Which.As<INavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Index == ChildAToChildDId)
        .And.ContainSingle(nav => nav.Index == ChildAToChildFId);
  }

  [Fact]
  public void Include_ShouldIncludeAllNavigationsAlsoBackwards()
  {
    // Arrange
    Includer<ChildD> includer = new(ChildDEntity);

    // Act
    includer
      .Include($"{nameof(ChildD.ChildA)}.{nameof(ChildA.ChildF)}")
      .Include($"{nameof(ChildD.ChildA)}.{nameof(ChildA.Root)}.{nameof(Root.ChildB)}")
      .Include($"{nameof(ChildD.ChildA)}.{nameof(ChildA.Root)}.{nameof(Root.ChildC)}")
      .Include(nameof(ChildD.ChildE));

    // Assert
    includer.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Index == ChildDToChildEId)
      .And.ContainSingle(nav => nav.Index == ChildDToChildAId)
      .Which.As<INavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Index == ChildAToChildFId)
        .And.ContainSingle(nav => nav.Index == ChildAToRootId)
        .Which.As<INavigationNode>().Children.Should().HaveCount(2)
          .And.ContainSingle(nav => nav.Index == RootToChildBId)
          .And.ContainSingle(nav => nav.Index == RootToChildCId);
  }
}
