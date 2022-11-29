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
    public ICollection<ManyToMany>? MTMEntities { get; set; }
  }

  public class ManyToMany
  {
    public int Id { get; }
    public string? Name { get; }

    public ICollection<Root>? Roots { get; set; }
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

  public class ChildG
  {
    public int Id { get; }
    public string? Name { get; }
    public int MTMEntityId { get; }

    public ManyToMany? MTMEntity { get; }
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
    public static readonly IEntityModel ManyToManyEntity = CreateManyToManyEntity().Mocked();
    public static readonly IEntityModel ChildGEntity = CreateChildGEntity().Mocked();

    public static readonly ISimpleNavigationModel RootToChildANavigation = CreateRootToChildANavigation();
    public static readonly ISimpleNavigationModel RootToChildBNavigation = CreateRootToChildBNavigation();
    public static readonly ISimpleNavigationModel RootToChildCNavigation = CreateRootToChildCNavigation();
    public static readonly ISimpleNavigationModel ChildAToChildDNavigation = CreateChildAToChildDNavigation();
    public static readonly ISimpleNavigationModel ChildAToChildFNavigation = CreateChildAToChildFNavigation();
    public static readonly ISimpleNavigationModel ChildDToChildENavigation = CreateChildDToChildENavigation();
    public static readonly ISimpleNavigationModel ChildAToRootNavigation = CreateChildAToRootNavigation();
    public static readonly ISimpleNavigationModel ChildDToChildANavigation = CreateChildDToChildANavigation();
    public static readonly ISkipNavigationModel RootToManyToManyNavigation = CreateRootToManyToManyNavigation();
    public static readonly ISkipNavigationModel ManyToManyToRootNavigation = CreateManyToManyToRootNavigation();
    public static readonly ISimpleNavigationModel ChildGToManyToManyNavigation = CreateChildGToManyToManyNavigation();

    public const int RootToChildAId = 1;
    public const int RootToChildBId = 2;
    public const int RootToChildCId = 3;
    public const int ChildAToChildDId = 4;
    public const int ChildAToChildFId = 5;
    public const int ChildDToChildEId = 6;
    public const int ChildAToRootId = 7;
    public const int ChildDToChildAId = 8;
    public const int RootToManyToManyId = 9;
    public const int ManyToManyToRootId = 10;
    public const int ChildGToManyToManyId = 11;

    private static IEntityModel CreateRootEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(Root.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(Root))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(Root.Name))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty))
        .Setup(mock => mock
          .Setup(x => x.Navigations)
          .Returns(() => new INavigationModel[] {
            RootToChildANavigation,
            RootToChildBNavigation,
            RootToChildCNavigation,
            RootToManyToManyNavigation
          })));
    }

    private static IEntityModel CreateChildAEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(ChildA.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
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

      return EntityModelBuilder.Build(_ => _
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

      return EntityModelBuilder.Build(_ => _
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

      return EntityModelBuilder.Build(_ => _
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

      return EntityModelBuilder.Build(_ => _
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

      return EntityModelBuilder.Build(_ => _
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

    private static IEntityModel CreateManyToManyEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(ManyToMany.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ManyToMany))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(ManyToMany.Name))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty))
        .Setup(mock => mock
          .Setup(x => x.Navigations)
          .Returns(() => new[] {
            ManyToManyToRootNavigation
          })));
    }

    private static IEntityModel CreateChildGEntity()
    {
      IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(_ => _
        .SetColumnName(nameof(ChildG.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ChildG))
        .SetProperties(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetColumnName(nameof(ChildG.Name)))
          .Add(_ => _
            .SetColumnName(nameof(ChildG.MTMEntityId))))
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty))
        .Setup(mock => mock
          .Setup(x => x.Navigations)
          .Returns(() => new[]
          {
            ChildGToManyToManyNavigation
          })));
    }

    private static ISimpleNavigationModel CreateRootToChildANavigation()
    {
      return SimpleNavigationModelBuilder.Build(_ => _
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

    private static ISimpleNavigationModel CreateRootToChildBNavigation()
    {
      return SimpleNavigationModelBuilder.Build(_ => _
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

    private static ISimpleNavigationModel CreateRootToChildCNavigation()
    {
      return SimpleNavigationModelBuilder.Build(_ => _
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

    private static ISimpleNavigationModel CreateChildAToChildDNavigation()
    {
      return SimpleNavigationModelBuilder.Build(_ => _
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

    private static ISimpleNavigationModel CreateChildAToChildFNavigation()
    {
      return SimpleNavigationModelBuilder.Build(_ => _
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

    private static ISimpleNavigationModel CreateChildDToChildENavigation()
    {
      return SimpleNavigationModelBuilder.Build(_ => _
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

    private static ISimpleNavigationModel CreateChildAToRootNavigation()
    {
      return SimpleNavigationModelBuilder.Build(_ => _
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

    private static ISimpleNavigationModel CreateChildDToChildANavigation()
    {
      return SimpleNavigationModelBuilder.Build(_ => _
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

    private static ISkipNavigationModel CreateRootToManyToManyNavigation()
    {
      return SkipNavigationModelBuilder.Build(_ => _
        .SetId(RootToManyToManyId)
        .SetTo(ManyToManyEntity)
        .SetName(nameof(Root.MTMEntities))
        .SetJoinTableName("RootManyToMany")
        .SetFromKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(Root.Id)))
            .SetToProperty(_ => _
              .SetColumnName("RootId"))))
        .SetToKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName("ManyToManyId"))
            .SetToProperty(_ => _
              .SetColumnName(nameof(ManyToMany.Id))))));
    }

    private static ISkipNavigationModel CreateManyToManyToRootNavigation()
    {
      return SkipNavigationModelBuilder.Build(_ => _
        .SetId(ManyToManyToRootId)
        .SetTo(RootEntity)
        .SetName(nameof(ManyToMany.Roots))
        .SetJoinTableName("RootManyToMany")
        .SetFromKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(ManyToMany.Id)))
            .SetToProperty(_ => _
              .SetColumnName("ManyToManyId"))))
        .SetToKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName("RootId"))
            .SetToProperty(_ => _
              .SetColumnName(nameof(Root.Id))))));
    }

    private static ISimpleNavigationModel CreateChildGToManyToManyNavigation()
    {
      return SimpleNavigationModelBuilder.Build(_ => _
        .SetId(ChildGToManyToManyId)
        .SetTo(ManyToManyEntity)
        .SetName(nameof(ChildG.MTMEntity))
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(ChildG.MTMEntityId)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(ManyToMany.Id))))));
    }
  }
}
