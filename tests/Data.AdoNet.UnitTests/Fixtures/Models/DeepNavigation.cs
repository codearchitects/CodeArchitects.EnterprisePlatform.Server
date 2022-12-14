using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.FluentMock;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Fixtures.Models;

internal static class DeepNavigation
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
    public static readonly IEntityModel<Root, int> RootEntity = CreateRootEntity().Mocked<Root, int>();
    public static readonly IEntityModel<ChildA, int> ChildAEntity = CreateChildAEntity().Mocked<ChildA, int>();
    public static readonly IEntityModel<ChildB, int> ChildBEntity = CreateChildBEntity().Mocked<ChildB, int>();
    public static readonly IEntityModel<ChildC, int> ChildCEntity = CreateChildCEntity().Mocked<ChildC, int>();
    public static readonly IEntityModel<ChildD, int> ChildDEntity = CreateChildDEntity().Mocked<ChildD, int>();
    public static readonly IEntityModel<ChildE, int> ChildEEntity = CreateChildEEntity().Mocked<ChildE, int>();
    public static readonly IEntityModel<ChildF, int> ChildFEntity = CreateChildFEntity().Mocked<ChildF, int>();
    public static readonly IEntityModel<ManyToMany, int> ManyToManyEntity = CreateManyToManyEntity().Mocked<ManyToMany, int>();
    public static readonly IEntityModel<ChildG, int> ChildGEntity = CreateChildGEntity().Mocked<ChildG, int>();

    public static readonly IAccessibleSimpleNavigationModel RootToChildANavigation = CreateRootToChildANavigation();
    public static readonly IAccessibleSimpleNavigationModel RootToChildBNavigation = CreateRootToChildBNavigation();
    public static readonly IAccessibleSimpleNavigationModel RootToChildCNavigation = CreateRootToChildCNavigation();
    public static readonly IAccessibleSimpleNavigationModel ChildAToChildDNavigation = CreateChildAToChildDNavigation();
    public static readonly IAccessibleSimpleNavigationModel ChildAToChildFNavigation = CreateChildAToChildFNavigation();
    public static readonly IAccessibleSimpleNavigationModel ChildDToChildENavigation = CreateChildDToChildENavigation();
    public static readonly IAccessibleSimpleNavigationModel ChildAToRootNavigation = CreateChildAToRootNavigation();
    public static readonly IAccessibleSimpleNavigationModel ChildDToChildANavigation = CreateChildDToChildANavigation();
    public static readonly IAccessibleSkipNavigationModel RootToManyToManyNavigation = CreateRootToManyToManyNavigation();
    public static readonly IAccessibleSkipNavigationModel ManyToManyToRootNavigation = CreateManyToManyToRootNavigation();
    public static readonly IAccessibleSimpleNavigationModel ChildGToManyToManyNavigation = CreateChildGToManyToManyNavigation();

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
      IPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetName(nameof(Root.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(Root))
        .SetColumns(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetName(nameof(Root.Name))))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty))
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
      IPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetName(nameof(ChildA.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ChildA))
        .SetColumns(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetName(nameof(ChildA.Name)))
          .Add(_ => _
            .SetName(nameof(ChildA.RootId))))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty))
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
      IPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetName(nameof(ChildB.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ChildB))
        .SetColumns(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetName(nameof(ChildB.Name)))
          .Add(_ => _
            .SetName(nameof(ChildB.RootId))))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty)));
    }

    private static IEntityModel CreateChildCEntity()
    {
      IPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetName(nameof(ChildC.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ChildC))
        .SetColumns(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetName(nameof(ChildC.Name)))
          .Add(_ => _
            .SetName(nameof(ChildC.RootId))))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty)));
    }

    private static IEntityModel CreateChildDEntity()
    {
      IPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetName(nameof(ChildD.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ChildD))
        .SetColumns(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetName(nameof(ChildD.Name)))
          .Add(_ => _
            .SetName(nameof(ChildD.ChildAId))))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty))
        .Setup(mock => mock
          .Setup(x => x.Navigations)
          .Returns(() => new[] {
            ChildDToChildANavigation,
            ChildDToChildENavigation
          })));
    }

    private static IEntityModel CreateChildEEntity()
    {
      IPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetName(nameof(ChildE.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ChildE))
        .SetColumns(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetName(nameof(ChildE.Name)))
          .Add(_ => _
            .SetName(nameof(ChildE.ChildDId))))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty)));
    }

    private static IEntityModel CreateChildFEntity()
    {
      IPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetName(nameof(ChildF.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ChildF))
        .SetColumns(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetName(nameof(ChildF.Name)))
          .Add(_ => _
            .SetName(nameof(ChildF.ChildAId))))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty)));
    }

    private static IEntityModel CreateManyToManyEntity()
    {
      IPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetName(nameof(ManyToMany.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ManyToMany))
        .SetColumns(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetName(nameof(ManyToMany.Name))))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty))
        .Setup(mock => mock
          .Setup(x => x.Navigations)
          .Returns(() => new[] {
            ManyToManyToRootNavigation
          })));
    }

    private static IEntityModel CreateChildGEntity()
    {
      IPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetName(nameof(ChildG.Id))
        .SetIndex(0));

      return EntityModelBuilder.Build(_ => _
        .SetTableName(nameof(ChildG))
        .SetColumns(_ => _
          .Add(idProperty)
          .Add(_ => _
            .SetName(nameof(ChildG.Name)))
          .Add(_ => _
            .SetName(nameof(ChildG.MTMEntityId))))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty))
        .Setup(mock => mock
          .Setup(x => x.Navigations)
          .Returns(() => new[]
          {
            ChildGToManyToManyNavigation
          })));
    }

    private static IAccessibleSimpleNavigationModel CreateRootToChildANavigation()
    {
      PropertyInfo propertyInfo = typeof(Root).GetRequiredProperty(nameof(Root.ChildA));

      return AccessibleSimpleNavigationModelBuilder.Build(_ => _
        .SetId(RootToChildAId)
        .SetTo(ChildAEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(Root.Id)))
            .SetToColumn(_ => _
              .SetName(nameof(ChildA.RootId))))));
    }

    private static IAccessibleSimpleNavigationModel CreateRootToChildBNavigation()
    {
      PropertyInfo propertyInfo = typeof(Root).GetRequiredProperty(nameof(Root.ChildB));

      return AccessibleSimpleNavigationModelBuilder.Build(_ => _
        .SetId(RootToChildBId)
        .SetTo(ChildBEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(Root.Id)))
            .SetToColumn(_ => _
              .SetName(nameof(ChildB.RootId))))));
    }

    private static IAccessibleSimpleNavigationModel CreateRootToChildCNavigation()
    {
      PropertyInfo propertyInfo = typeof(Root).GetRequiredProperty(nameof(Root.ChildC));

      return AccessibleSimpleNavigationModelBuilder.Build(_ => _
        .SetId(RootToChildCId)
        .SetTo(ChildCEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(Root.Id)))
            .SetToColumn(_ => _
              .SetName(nameof(ChildC.RootId))))));
    }

    private static IAccessibleSimpleNavigationModel CreateChildAToChildDNavigation()
    {
      PropertyInfo propertyInfo = typeof(ChildA).GetRequiredProperty(nameof(ChildA.ChildD));

      return AccessibleSimpleNavigationModelBuilder.Build(_ => _
        .SetId(ChildAToChildDId)
        .SetTo(ChildDEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(ChildA.Id)))
            .SetToColumn(_ => _
              .SetName(nameof(ChildD.ChildAId))))));
    }

    private static IAccessibleSimpleNavigationModel CreateChildAToChildFNavigation()
    {
      PropertyInfo propertyInfo = typeof(ChildA).GetRequiredProperty(nameof(ChildA.ChildF));

      return AccessibleSimpleNavigationModelBuilder.Build(_ => _
        .SetId(ChildAToChildFId)
        .SetTo(ChildFEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(ChildA.Id)))
            .SetToColumn(_ => _
              .SetName(nameof(ChildF.ChildAId))))));
    }

    private static IAccessibleSimpleNavigationModel CreateChildDToChildENavigation()
    {
      PropertyInfo propertyInfo = typeof(ChildD).GetRequiredProperty(nameof(ChildD.ChildE));

      return AccessibleSimpleNavigationModelBuilder.Build(_ => _
        .SetId(ChildDToChildEId)
        .SetTo(ChildEEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(ChildD.Id)))
            .SetToColumn(_ => _
              .SetName(nameof(ChildE.ChildDId))))));
    }

    private static IAccessibleSimpleNavigationModel CreateChildAToRootNavigation()
    {
      PropertyInfo propertyInfo = typeof(ChildA).GetRequiredProperty(nameof(ChildA.Root));

      return AccessibleSimpleNavigationModelBuilder.Build(_ => _
        .SetId(ChildAToRootId)
        .SetTo(RootEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(ChildA.RootId)))
            .SetToColumn(_ => _
              .SetName(nameof(Root.Id))))));
    }

    private static IAccessibleSimpleNavigationModel CreateChildDToChildANavigation()
    {
      PropertyInfo propertyInfo = typeof(ChildD).GetRequiredProperty(nameof(ChildD.ChildA));

      return AccessibleSimpleNavigationModelBuilder.Build(_ => _
        .SetId(ChildDToChildAId)
        .SetTo(ChildAEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(ChildD.ChildAId)))
            .SetToColumn(_ => _
              .SetName(nameof(ChildA.Id))))));
    }

    private static IAccessibleSkipNavigationModel CreateRootToManyToManyNavigation()
    {
      PropertyInfo propertyInfo = typeof(Root).GetRequiredProperty(nameof(Root.MTMEntities));

      return AccessibleSkipNavigationModelBuilder.Build(_ => _
        .SetId(RootToManyToManyId)
        .SetTo(ManyToManyEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetJoinEntity(_ => _
          .SetTableName("RootManyToMany"))
        .SetFromKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(Root.Id)))
            .SetToColumn(_ => _
              .SetName("RootId"))))
        .SetToKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName("ManyToManyId"))
            .SetToColumn(_ => _
              .SetName(nameof(ManyToMany.Id))))));
    }

    private static IAccessibleSkipNavigationModel CreateManyToManyToRootNavigation()
    {
      PropertyInfo propertyInfo = typeof(ManyToMany).GetRequiredProperty(nameof(ManyToMany.Roots));

      return AccessibleSkipNavigationModelBuilder.Build(_ => _
        .SetId(ManyToManyToRootId)
        .SetTo(RootEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetJoinEntity(_ => _
          .SetTableName("RootManyToMany"))
        .SetFromKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(ManyToMany.Id)))
            .SetToColumn(_ => _
              .SetName("ManyToManyId"))))
        .SetToKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName("RootId"))
            .SetToColumn(_ => _
              .SetName(nameof(Root.Id))))));
    }

    private static IAccessibleSimpleNavigationModel CreateChildGToManyToManyNavigation()
    {
      PropertyInfo propertyInfo = typeof(ChildG).GetRequiredProperty(nameof(ChildG.MTMEntity));

      return AccessibleSimpleNavigationModelBuilder.Build(_ => _
        .SetId(ChildGToManyToManyId)
        .SetTo(ManyToManyEntity)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(propertyInfo))
        .SetKeyPairs(_ => _
          .Add(_ => _
            .SetFromColumn(_ => _
              .SetName(nameof(ChildG.MTMEntityId)))
            .SetToColumn(_ => _
              .SetName(nameof(ManyToMany.Id))))));
    }
  }
}
