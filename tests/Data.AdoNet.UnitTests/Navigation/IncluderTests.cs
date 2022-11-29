using static CodeArchitects.Platform.Data.AdoNet.Fixtures.NavigationFixture;
using static CodeArchitects.Platform.Data.AdoNet.Fixtures.NavigationFixture.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

public class IncluderTests
{
  [Fact]
  public void IncludeLiteral_ShouldIncludeLeafNavigation_WhenDepth1()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include(nameof(Root.ChildA));

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeLeafNavigation_WhenDepth1()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include(root => root.ChildA);

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeMultipleLeafNavigations_WhenDepth1()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(nameof(Root.ChildA))
      .Include(nameof(Root.ChildB));

    // Assert
    includer.Root.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeMultipleLeafNavigations_WhenDepth1()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA)
      .Include(root => root.ChildB);

    // Assert
    includer.Root.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeObj_ShouldIncludeMultipleLeafNavigations_WhenDepth1()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include(root => new { root.ChildA, root.ChildB });

    // Assert
    includer.Root.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeLeafNavigation_WhenDepth1AndIncludedMultipleTimes()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(nameof(Root.ChildA))
      .Include(nameof(Root.ChildA));

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeLeafNavigation_WhenDepth1AndIncludedMultipleTimes()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA)
      .Include(root => root.ChildA);

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeNodeNavigation_WhenDepth2()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}");

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeNodeNavigation_WhenDepth2()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include(root => root.ChildA!.ChildD);

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeThen_ShouldIncludeNodeNavigation_WhenDepth2()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include(root => root.ChildA, includer => includer
      .Include(childA => childA.ChildD));

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsLeaf()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(nameof(Root.ChildA))
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}");

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsLeaf()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA)
      .Include(root => root.ChildA!.ChildD);

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsNode()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}")
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildF)}");

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsNode()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA!.ChildD)
      .Include(root => root.ChildA!.ChildF);

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeObj_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsNode()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include(root => new { root.ChildA!.ChildD, root.ChildA!.ChildF });

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeThen_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsNode()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA, _ => _
        .Include(childA => childA.ChildD))
      .Include(root => root.ChildA, _ => _
        .Include(childA => childA.ChildF));

    // Assert
    includer.Root.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeAllNavigations()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}.{nameof(ChildD.ChildE)}")
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildF)}")
      .Include(nameof(Root.ChildB))
      .Include(nameof(Root.ChildC));

    // Assert
    includer.Root.Navigations.Should().HaveCount(3)
      .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildCId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleNode)
        .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(1)
          .And.ContainSingle(nav => nav.Id == ChildDToChildEId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeAllNavigations()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA!.ChildD!.ChildE)
      .Include(root => root.ChildA!.ChildF)
      .Include(root => root.ChildB)
      .Include(root => root.ChildC);

    // Assert
    includer.Root.Navigations.Should().HaveCount(3)
      .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildCId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleNode)
        .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(1)
          .And.ContainSingle(nav => nav.Id == ChildDToChildEId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeObj_ShouldIncludeAllNavigations()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer.Include(root => new { root.ChildA!.ChildD!.ChildE, root.ChildA!.ChildF, root.ChildB, root.ChildC });

    // Assert
    includer.Root.Navigations.Should().HaveCount(3)
      .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildCId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleNode)
        .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(1)
          .And.ContainSingle(nav => nav.Id == ChildDToChildEId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeThen_ShouldIncludeAllNavigations()
  {
    // Arrange
    Includer<Root> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA, _ => _
        .Include(childA => childA.ChildD, _ => _
          .Include(childD => childD.ChildE))
        .Include(childA => childA.ChildF))
      .Include(root => root.ChildB)
      .Include(root => root.ChildC);

    // Assert
    includer.Root.Navigations.Should().HaveCount(3)
      .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildCId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == RootToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToChildDId && nav is INavigationSimpleNode)
        .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(1)
          .And.ContainSingle(nav => nav.Id == ChildDToChildEId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeAllNavigationsAlsoBackwards()
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
    includer.Root.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Id == ChildDToChildEId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == ChildDToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToRootId && nav is INavigationSimpleNode)
        .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
          .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf)
          .And.ContainSingle(nav => nav.Id == RootToChildCId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeAllNavigationsAlsoBackwards()
  {
    // Arrange
    Includer<ChildD> includer = new(ChildDEntity);

    // Act
    includer
      .Include(childD => childD.ChildA!.ChildF)
      .Include(childD => childD.ChildA!.Root!.ChildB)
      .Include(childD => childD.ChildA!.Root!.ChildC)
      .Include(childD => childD.ChildE);

    // Assert
    includer.Root.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Id == ChildDToChildEId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == ChildDToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToRootId && nav is INavigationSimpleNode)
        .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
          .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf)
          .And.ContainSingle(nav => nav.Id == RootToChildCId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeObj_ShouldIncludeAllNavigationsAlsoBackwards()
  {
    // Arrange
    Includer<ChildD> includer = new(ChildDEntity);

    // Act
    includer.Include(childD => new { childD.ChildA!.ChildF, childD.ChildA!.Root!.ChildB, childD.ChildA!.Root!.ChildC, childD.ChildE });

    // Assert
    includer.Root.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Id == ChildDToChildEId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == ChildDToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToRootId && nav is INavigationSimpleNode)
        .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
          .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf)
          .And.ContainSingle(nav => nav.Id == RootToChildCId && nav is INavigationSimpleLeaf);
  }

  [Fact]
  public void IncludeThen_ShouldIncludeAllNavigationsAlsoBackwards()
  {
    // Arrange
    Includer<ChildD> includer = new(ChildDEntity);

    // Act
    includer
      .Include(childD => childD.ChildA, _ => _
        .Include(childA => childA.ChildF)
        .Include(childA => childA.Root, _ => _
          .Include(root => root.ChildB)
          .Include(root => root.ChildC)))
      .Include(childD => childD.ChildE);

    // Assert
    includer.Root.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Id == ChildDToChildEId && nav is INavigationSimpleLeaf)
      .And.ContainSingle(nav => nav.Id == ChildDToChildAId && nav is INavigationSimpleNode)
      .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Id == ChildAToChildFId && nav is INavigationSimpleLeaf)
        .And.ContainSingle(nav => nav.Id == ChildAToRootId && nav is INavigationSimpleNode)
        .Which.As<INavigationSimpleNode>().Children.Should().HaveCount(2)
          .And.ContainSingle(nav => nav.Id == RootToChildBId && nav is INavigationSimpleLeaf)
          .And.ContainSingle(nav => nav.Id == RootToChildCId && nav is INavigationSimpleLeaf);
  }
}
