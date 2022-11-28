using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.FluentMock;

namespace CodeArchitects.Platform.Data.AdoNet.Fixtures;

internal static class NavigationFixture
{
  public class Root
  {
    public int Id { get; }
    public string? Name { get; }

    public ChildA? ChildA { get; }
    public ChildB? ChildB { get; }
    public ChildC? ChildC { get; }
  }

  public class ChildA
  {
    public int Id { get; }
    public string? Name { get; }
    public int RootId { get; }

    public Root? Root { get; }
    public ChildD? ChildD { get; }
    public ChildF? ChildF { get; }
  }

  public class ChildB
  {
    public int Id { get; }
    public string? Name { get; }
    public int RootId { get; }

    public Root? Root { get; }
  }

  public class ChildC
  {
    public int Id { get; }
    public string? Name { get; }
    public int RootId { get; }

    public Root? Root { get; }
  }

  public class ChildD
  {
    public int Id { get; }
    public string? Name { get; }
    public int ChildAId { get; }

    public ChildA? ChildA { get; }
    public ChildE? ChildE { get; }
  }

  public class ChildE
  {
    public int Id { get; }
    public string? Name { get; }
    public int ChildDId { get; }

    public ChildD? ChildD { get; }
  }

  public class ChildF
  {
    public int Id { get; }
    public string? Name { get; }
    public int ChildAId { get; }

    public ChildA? ChildA { get; }
  }

  public static class Model
  {
    public static readonly IEntityModel RootEntity = CreateRootEntity().Mocked();
    public static readonly IEntityModel ChildAEntity = CreateChildAEntity().Mocked();
    public static readonly IEntityModel ChildBEntity = CreateChildBEntity().Mocked();
    public static readonly IEntityModel ChildCEntity = CreateChildCEntity().Mocked();
    public static readonly IEntityModel ChildDEntity = CreateChildDEntity().Mocked();
    public static readonly IEntityModel ChildEEntity = CreateChildEEntity().Mocked();
    public static readonly IEntityModel ChildFEntity = CreateChildFEntity().Mocked();

    public static readonly INavigationModel RootToChildANavigation = CreateRootToChildANavigation();
    public static readonly INavigationModel RootToChildBNavigation = CreateRootToChildBNavigation();
    public static readonly INavigationModel RootToChildCNavigation = CreateRootToChildCNavigation();
    public static readonly INavigationModel ChildAToChildDNavigation = CreateChildAToChildDNavigation();
    public static readonly INavigationModel ChildAToChildFNavigation = CreateChildAToChildFNavigation();
    public static readonly INavigationModel ChildDToChildENavigation = CreateChildDToChildENavigation();
    public static readonly INavigationModel ChildAToRootNavigation = CreateChildAToRootNavigation();
    public static readonly INavigationModel ChildDToChildANavigation = CreateChildDToChildANavigation();

    public const int RootToChildAId = 1;
    public const int RootToChildBId = 2;
    public const int RootToChildCId = 3;
    public const int ChildAToChildDId = 4;
    public const int ChildAToChildFId = 5;
    public const int ChildDToChildEId = 6;
    public const int ChildAToRootId = 7;
    public const int ChildDToChildAId = 8;

    private static IEntityModel CreateRootEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(Root.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetTableName(nameof(Root))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(Root.Name))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty))
        .Setup(mock => mock
          .Setup(x => x.Navigations)
          .Returns(() => new[] {
            RootToChildANavigation,
            RootToChildBNavigation,
            RootToChildCNavigation
          })));
    }

    private static IEntityModel CreateChildAEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(ChildA.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetTableName(nameof(ChildA))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(ChildA.Name)))
          .Add(_ => _
            .SetColumnName(nameof(ChildA.RootId))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty))
        .Setup(mock => mock
          .Setup(x => x.Navigations)
          .Returns(() => new[] {
            ChildAToRootNavigation,
            ChildAToChildDNavigation,
            ChildAToChildFNavigation
          })));
    }

    private static IEntityModel CreateChildBEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(ChildB.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetTableName(nameof(ChildB))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(ChildB.Name)))
          .Add(_ => _
            .SetColumnName(nameof(ChildB.RootId))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty)));
    }

    private static IEntityModel CreateChildCEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(ChildC.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetTableName(nameof(ChildC))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(ChildC.Name)))
          .Add(_ => _
            .SetColumnName(nameof(ChildC.RootId))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty)));
    }

    private static IEntityModel CreateChildDEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(ChildD.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetTableName(nameof(ChildD))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(ChildD.Name)))
          .Add(_ => _
            .SetColumnName(nameof(ChildD.ChildAId))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty))
        .Setup(mock => mock
          .Setup(x => x.Navigations)
          .Returns(() => new[] {
            ChildDToChildANavigation,
            ChildDToChildENavigation
          })));
    }

    private static IEntityModel CreateChildEEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(ChildE.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetTableName(nameof(ChildE))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(ChildE.Name)))
          .Add(_ => _
            .SetColumnName(nameof(ChildE.ChildDId))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty)));
    }

    private static IEntityModel CreateChildFEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(ChildF.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetTableName(nameof(ChildF))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(ChildF.Name)))
          .Add(_ => _
            .SetColumnName(nameof(ChildF.ChildAId))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty)));
    }

    private static INavigationModel CreateRootToChildANavigation()
    {
      return NavigationModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetId(RootToChildAId)
        .SetTo(ChildAEntity)
        .SetName(nameof(Root.ChildA))
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(Root.Id)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(ChildA.RootId))))));
    }

    private static INavigationModel CreateRootToChildBNavigation()
    {
      return NavigationModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetId(RootToChildBId)
        .SetTo(ChildBEntity)
        .SetName(nameof(Root.ChildB))
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(Root.Id)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(ChildB.RootId))))));
    }

    private static INavigationModel CreateRootToChildCNavigation()
    {
      return NavigationModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetId(RootToChildCId)
        .SetTo(ChildCEntity)
        .SetName(nameof(Root.ChildC))
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(Root.Id)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(ChildC.RootId))))));
    }

    private static INavigationModel CreateChildAToChildDNavigation()
    {
      return NavigationModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetId(ChildAToChildDId)
        .SetTo(ChildDEntity)
        .SetName(nameof(ChildA.ChildD))
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(ChildA.Id)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(ChildD.ChildAId))))));
    }

    private static INavigationModel CreateChildAToChildFNavigation()
    {
      return NavigationModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetId(ChildAToChildFId)
        .SetTo(ChildFEntity)
        .SetName(nameof(ChildA.ChildF))
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(ChildA.Id)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(ChildF.ChildAId))))));
    }

    private static INavigationModel CreateChildDToChildENavigation()
    {
      return NavigationModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetId(ChildDToChildEId)
        .SetTo(ChildEEntity)
        .SetName(nameof(ChildD.ChildE))
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(ChildD.Id)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(ChildE.ChildDId))))));
    }

    private static INavigationModel CreateChildAToRootNavigation()
    {
      return NavigationModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetId(ChildAToRootId)
        .SetTo(RootEntity)
        .SetName(nameof(ChildA.Root))
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(ChildA.RootId)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(Root.Id))))));
    }

    private static INavigationModel CreateChildDToChildANavigation()
    {
      return NavigationModelBuilder.Build(MockBehavior.Strict, _ => _
        .SetId(ChildDToChildAId)
        .SetTo(ChildAEntity)
        .SetName(nameof(ChildD.ChildA))
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(ChildD.ChildAId)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(ChildA.Id))))));
    }
  }
}
