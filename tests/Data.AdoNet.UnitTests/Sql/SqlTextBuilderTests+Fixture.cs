using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.FluentMock;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

public partial class SqlTextBuilderTests
{
  private class Root
  {
    public int Id { get; }
    public string? Name { get; }

    public ChildA? ChildA { get; }
    public ChildB? ChildB { get; }
    public ChildC? ChildC { get; }
  }

  private class ChildA
  {
    public int Id { get; }
    public string? Name { get; }
    public int RootId { get; }

    public Root? Root { get; }
    public ChildD? ChildD { get; }
    public ChildF? ChildF { get; }
  }

  private class ChildB
  {
    public int Id { get; }
    public string? Name { get; }
    public int RootId { get; }

    public Root? Root { get; }
  }

  private class ChildC
  {
    public int Id { get; }
    public string? Name { get; }
    public int RootId { get; }

    public Root? Root { get; }
  }

  private class ChildD
  {
    public int Id { get; }
    public string? Name { get; }
    public int ChildAId { get; }

    public ChildA? ChildA { get; }
    public ChildE? ChildE { get; }
  }

  private class ChildE
  {
    public int Id { get; }
    public string? Name { get; }
    public int ChildDId { get; }

    public ChildD? ChildD { get; }
  }

  private class ChildF
  {
    public int Id { get; }
    public string? Name { get; }
    public int ChildAId { get; }

    public ChildA? ChildA { get; }
  }

  private static class Model
  {
    public static readonly IEntityModel RootEntity = CreateRootEntity();
    public static readonly IEntityModel ChildAEntity = CreateChildAEntity();
    public static readonly IEntityModel ChildBEntity = CreateChildBEntity();
    public static readonly IEntityModel ChildCEntity = CreateChildCEntity();
    public static readonly IEntityModel ChildDEntity = CreateChildDEntity();
    public static readonly IEntityModel ChildEEntity = CreateChildEEntity();
    public static readonly IEntityModel ChildFEntity = CreateChildFEntity();

    public static readonly INavigationModel RootToChildANavigation = CreateRootToChildANavigation();
    public static readonly INavigationModel RootToChildBNavigation = CreateRootToChildBNavigation();
    public static readonly INavigationModel RootToChildCNavigation = CreateRootToChildCNavigation();
    public static readonly INavigationModel ChildAToChildDNavigation = CreateChildAToChildDNavigation();
    public static readonly INavigationModel ChildAToChildFNavigation = CreateChildAToChildFNavigation();
    public static readonly INavigationModel ChildDToChildENavigation = CreateChildDToChildENavigation();

    public static readonly INavigationModel ChildAToRootNavigation = CreateChildAToRootNavigation();

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
          .SetProperties(idProperty)));
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
          .SetProperties(idProperty)));
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
          .SetProperties(idProperty)));
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
        .SetId(1)
        .SetTo(ChildAEntity)
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
        .SetId(2)
        .SetTo(ChildBEntity)
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
        .SetId(3)
        .SetTo(ChildCEntity)
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
        .SetId(4)
        .SetTo(ChildDEntity)
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
        .SetId(5)
        .SetTo(ChildFEntity)
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
        .SetId(6)
        .SetTo(ChildEEntity)
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
        .SetId(7)
        .SetTo(RootEntity)
        .SetKeys(_ => _
          .Add(_ => _
            .SetFromProperty(_ => _
              .SetColumnName(nameof(ChildA.RootId)))
            .SetToProperty(_ => _
              .SetColumnName(nameof(Root.Id))))));
    }
  }


  private record NavigationLeaf(INavigationModel Model) : INavigationLeaf
  {
    public int Index => Model.Id;

    public IEntityModel Target => Model.To;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
      where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitLeaf(this);
    }

    public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
      where TVisitor : INavigationVisitor<TResult, TState>
    {
      return visitor.VisitLeaf(this, in state);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }

  private record NavigationNode(INavigationModel Model, IReadOnlyList<INavigation> Children) : INavigationNode
  {
    public int Index => Model.Id;

    public IEntityModel Target => Model.To;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor) where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitNode(this);
    }

    public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state) where TVisitor : INavigationVisitor<TResult, TState>
    {
      return visitor.VisitNode(this, in state);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }
}
