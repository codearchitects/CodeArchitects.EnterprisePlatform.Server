using static CodeArchitects.Platform.Data.AdoNet.Fixtures.Models.DeepNavigation;
using static CodeArchitects.Platform.Data.AdoNet.Fixtures.Models.DeepNavigation.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

public class IncluderTests
{
  [Fact]
  public void IncludeLiteral_ShouldIncludeLeafNavigation_WhenDepth1()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer.Include(nameof(Root.ChildA));

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeLeafNavigation_WhenDepth1()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer.Include(root => root.ChildA);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeMultipleLeafNavigations_WhenDepth1()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(nameof(Root.ChildA))
      .Include(nameof(Root.ChildB));

    // Assert
    includer.Spec.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeMultipleLeafNavigations_WhenDepth1()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA)
      .Include(root => root.ChildB);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeObj_ShouldIncludeMultipleLeafNavigations_WhenDepth1()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer.Include(root => new { root.ChildA, root.ChildB });

    // Assert
    includer.Spec.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeLeafNavigation_WhenDepth1AndIncludedMultipleTimes()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(nameof(Root.ChildA))
      .Include(nameof(Root.ChildA));

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeLeafNavigation_WhenDepth1AndIncludedMultipleTimes()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA)
      .Include(root => root.ChildA);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeNodeNavigation_WhenDepth2()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer.Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}");

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeNodeNavigation_WhenDepth2()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer.Include(root => root.ChildA!.ChildD);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeThen_ShouldIncludeNodeNavigation_WhenDepth2()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer.Include(root => root.ChildA, includer => includer
      .Include(childA => childA.ChildD));

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsLeaf()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(nameof(Root.ChildA))
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}");

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsLeaf()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA)
      .Include(root => root.ChildA!.ChildD);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(1)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsNode()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}")
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildF)}");

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsNode()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA!.ChildD)
      .Include(root => root.ChildA!.ChildF);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeObj_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsNode()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer.Include(root => new { root.ChildA!.ChildD, root.ChildA!.ChildF });

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeThen_ShouldIncludeNodeNavigation_WhenDepth2AndIncludedFirstTimeAsNode()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA, _ => _
        .Include(childA => childA.ChildD))
      .Include(root => root.ChildA, _ => _
        .Include(childA => childA.ChildF));

    // Assert
    includer.Spec.Navigations.Should().HaveCount(1)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeAllNavigations()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildD)}.{nameof(ChildD.ChildE)}")
      .Include($"{nameof(Root.ChildA)}.{nameof(ChildA.ChildF)}")
      .Include(nameof(Root.ChildB))
      .Include(nameof(Root.ChildC));

    // Assert
    includer.Spec.Navigations.Should().HaveCount(3)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildCId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationNode)
        .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(1)
          .And.ContainSingle(nav => nav.Model.Id == ChildDToChildEId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeAllNavigations()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA!.ChildD!.ChildE)
      .Include(root => root.ChildA!.ChildF)
      .Include(root => root.ChildB)
      .Include(root => root.ChildC);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(3)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildCId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationNode)
        .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(1)
          .And.ContainSingle(nav => nav.Model.Id == ChildDToChildEId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeObj_ShouldIncludeAllNavigations()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer.Include(root => new { root.ChildA!.ChildD!.ChildE, root.ChildA!.ChildF, root.ChildB, root.ChildC });

    // Assert
    includer.Spec.Navigations.Should().HaveCount(3)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildCId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationNode)
        .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(1)
          .And.ContainSingle(nav => nav.Model.Id == ChildDToChildEId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeThen_ShouldIncludeAllNavigations()
  {
    // Arrange
    RootIncluder<Root, int> includer = new(RootEntity);

    // Act
    includer
      .Include(root => root.ChildA, _ => _
        .Include(childA => childA.ChildD, _ => _
          .Include(childD => childD.ChildE))
        .Include(childA => childA.ChildF))
      .Include(root => root.ChildB)
      .Include(root => root.ChildC);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(3)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildCId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == RootToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildDId && nav is ISimpleNavigationNode)
        .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(1)
          .And.ContainSingle(nav => nav.Model.Id == ChildDToChildEId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeLiteral_ShouldIncludeAllNavigationsAlsoBackwards()
  {
    // Arrange
    RootIncluder<ChildD, int> includer = new(ChildDEntity);

    // Act
    includer
      .Include($"{nameof(ChildD.ChildA)}.{nameof(ChildA.ChildF)}")
      .Include($"{nameof(ChildD.ChildA)}.{nameof(ChildA.Root)}.{nameof(Root.ChildB)}")
      .Include($"{nameof(ChildD.ChildA)}.{nameof(ChildA.Root)}.{nameof(Root.ChildC)}")
      .Include(nameof(ChildD.ChildE));

    // Assert
    includer.Spec.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Model.Id == ChildDToChildEId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == ChildDToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToRootId && nav is ISimpleNavigationNode)
        .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
          .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf)
          .And.ContainSingle(nav => nav.Model.Id == RootToChildCId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeExpr_ShouldIncludeAllNavigationsAlsoBackwards()
  {
    // Arrange
    RootIncluder<ChildD, int> includer = new(ChildDEntity);

    // Act
    includer
      .Include(childD => childD.ChildA!.ChildF)
      .Include(childD => childD.ChildA!.Root!.ChildB)
      .Include(childD => childD.ChildA!.Root!.ChildC)
      .Include(childD => childD.ChildE);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Model.Id == ChildDToChildEId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == ChildDToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToRootId && nav is ISimpleNavigationNode)
        .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
          .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf)
          .And.ContainSingle(nav => nav.Model.Id == RootToChildCId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeObj_ShouldIncludeAllNavigationsAlsoBackwards()
  {
    // Arrange
    RootIncluder<ChildD, int> includer = new(ChildDEntity);

    // Act
    includer.Include(childD => new { childD.ChildA!.ChildF, childD.ChildA!.Root!.ChildB, childD.ChildA!.Root!.ChildC, childD.ChildE });

    // Assert
    includer.Spec.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Model.Id == ChildDToChildEId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == ChildDToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToRootId && nav is ISimpleNavigationNode)
        .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
          .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf)
          .And.ContainSingle(nav => nav.Model.Id == RootToChildCId && nav is ISimpleNavigationLeaf);
  }

  [Fact]
  public void IncludeThen_ShouldIncludeAllNavigationsAlsoBackwards()
  {
    // Arrange
    RootIncluder<ChildD, int> includer = new(ChildDEntity);

    // Act
    includer
      .Include(childD => childD.ChildA, _ => _
        .Include(childA => childA.ChildF)
        .Include(childA => childA.Root, _ => _
          .Include(root => root.ChildB)
          .Include(root => root.ChildC)))
      .Include(childD => childD.ChildE);

    // Assert
    includer.Spec.Navigations.Should().HaveCount(2)
      .And.ContainSingle(nav => nav.Model.Id == ChildDToChildEId && nav is ISimpleNavigationLeaf)
      .And.ContainSingle(nav => nav.Model.Id == ChildDToChildAId && nav is ISimpleNavigationNode)
      .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToChildFId && nav is ISimpleNavigationLeaf)
        .And.ContainSingle(nav => nav.Model.Id == ChildAToRootId && nav is ISimpleNavigationNode)
        .Which.As<ISimpleNavigationNode>().Children.Should().HaveCount(2)
          .And.ContainSingle(nav => nav.Model.Id == RootToChildBId && nav is ISimpleNavigationLeaf)
          .And.ContainSingle(nav => nav.Model.Id == RootToChildCId && nav is ISimpleNavigationLeaf);
  }
}
