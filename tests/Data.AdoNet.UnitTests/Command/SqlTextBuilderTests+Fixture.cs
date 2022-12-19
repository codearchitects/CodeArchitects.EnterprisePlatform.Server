using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Oracle.Command;
using CodeArchitects.Platform.Data.AdoNet.PostgreSQL.Command;
using CodeArchitects.Platform.Data.AdoNet.SQLServer.Command;
using System.Reflection;
using Xunit.Sdk;
using static CodeArchitects.Platform.Data.AdoNet.Fixtures.Models.DeepNavigation.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

public partial class SqlTextBuilderTests
{
  public abstract class SelectTextDataAttribute : DataAttribute
  {
    private protected abstract NavigationSpec Spec { get; }

    protected abstract string ExpectedTextSQLServer { get; }

    protected abstract string ExpectedTextPostgreSQL { get; }

    protected abstract string ExpectedTextOracle { get; }

    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      ISqlTextCache cache = Mock.Of<ISqlTextCache>();

      yield return new object[] { new SqlTextBuilder(cache, new SQLServerSyntaxProvider()), Spec, ExpectedTextSQLServer };
      yield return new object[] { new SqlTextBuilder(cache, new PostgreSQLSyntaxProvider()), Spec, ExpectedTextPostgreSQL };
      yield return new object[] { new SqlTextBuilder(cache, new OracleSyntaxProvider()), Spec, ExpectedTextOracle };
    }
  }

  public class NoIncludeAttribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(RootEntity, Array.Empty<INavigation>());

    protected override string ExpectedTextSQLServer => """
      SELECT [Id], [Name]
      FROM [Root]
      WHERE [Id] = @p0
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = @p0
      """;

    protected override string ExpectedTextOracle => """
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = :p0
      """;
  }

  public class IncludeOneAttribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(RootEntity, new INavigation[]
    {
      new SimpleNavigationLeaf(RootToChildANavigation)
    });

    protected override string ExpectedTextSQLServer => """
      SELECT t.[Id], t.[Name], t1.[Id] AS [Id_1], t1.[Name] AS [Name_1], t1.[RootId] AS [RootId_1]
      FROM (
      SELECT [Id], [Name]
      FROM [Root]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN [ChildA] AS t1 ON t.[Id] = t1.[RootId]
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT t."Id", t."Name", t1."Id" AS "Id_1", t1."Name" AS "Name_1", t1."RootId" AS "RootId_1"
      FROM (
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = @p0
      ) AS t
      LEFT JOIN "ChildA" AS t1 ON t."Id" = t1."RootId"
      """;

    protected override string ExpectedTextOracle => """
      SELECT t."Id", t."Name", t1."Id" AS "Id_1", t1."Name" AS "Name_1", t1."RootId" AS "RootId_1"
      FROM (
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = :p0
      ) AS t
      LEFT JOIN "ChildA" AS t1 ON t."Id" = t1."RootId"
      """;
  }

  public class IncludeTwoAttribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(RootEntity, new INavigation[]
    {
      new SimpleNavigationLeaf(RootToChildANavigation),
      new SimpleNavigationLeaf(RootToChildBNavigation)
    });

    protected override string ExpectedTextSQLServer => """
      SELECT t.[Id], t.[Name], t1.[Id] AS [Id_1], t1.[Name] AS [Name_1], t1.[RootId] AS [RootId_1], t2.[Id] AS [Id_2], t2.[Name] AS [Name_2], t2.[RootId] AS [RootId_2]
      FROM (
      SELECT [Id], [Name]
      FROM [Root]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN [ChildA] AS t1 ON t.[Id] = t1.[RootId]
      LEFT JOIN [ChildB] AS t2 ON t.[Id] = t2.[RootId]
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT t."Id", t."Name", t1."Id" AS "Id_1", t1."Name" AS "Name_1", t1."RootId" AS "RootId_1", t2."Id" AS "Id_2", t2."Name" AS "Name_2", t2."RootId" AS "RootId_2"
      FROM (
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = @p0
      ) AS t
      LEFT JOIN "ChildA" AS t1 ON t."Id" = t1."RootId"
      LEFT JOIN "ChildB" AS t2 ON t."Id" = t2."RootId"
      """;

    protected override string ExpectedTextOracle => """
      SELECT t."Id", t."Name", t1."Id" AS "Id_1", t1."Name" AS "Name_1", t1."RootId" AS "RootId_1", t2."Id" AS "Id_2", t2."Name" AS "Name_2", t2."RootId" AS "RootId_2"
      FROM (
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = :p0
      ) AS t
      LEFT JOIN "ChildA" AS t1 ON t."Id" = t1."RootId"
      LEFT JOIN "ChildB" AS t2 ON t."Id" = t2."RootId"
      """;
  }

  public class IncludeManyToManyAsLeafAttribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(RootEntity, new INavigation[]
    {
      new SkipNavigationLeaf(RootToManyToManyNavigation)
    });

    protected override string ExpectedTextSQLServer => """
      SELECT t.[Id], t.[Name], t9.[Id] AS [Id_9], t9.[Name] AS [Name_9]
      FROM (
      SELECT [Id], [Name]
      FROM [Root]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN (
      SELECT t9.[Id], t9.[Name], t.[RootId]
      FROM [RootManyToMany] AS t
      INNER JOIN [ManyToMany] AS t9 ON t.[ManyToManyId] = t9.[Id]
      ) AS t9 ON t.[Id] = t9.[RootId]
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT t."Id", t."Name", t9."Id" AS "Id_9", t9."Name" AS "Name_9"
      FROM (
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = @p0
      ) AS t
      LEFT JOIN (
      SELECT t9."Id", t9."Name", t."RootId"
      FROM "RootManyToMany" AS t
      INNER JOIN "ManyToMany" AS t9 ON t."ManyToManyId" = t9."Id"
      ) AS t9 ON t."Id" = t9."RootId"
      """;

    protected override string ExpectedTextOracle => """
      SELECT t."Id", t."Name", t9."Id" AS "Id_9", t9."Name" AS "Name_9"
      FROM (
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = :p0
      ) AS t
      LEFT JOIN (
      SELECT t9."Id", t9."Name", t."RootId"
      FROM "RootManyToMany" AS t
      INNER JOIN "ManyToMany" AS t9 ON t."ManyToManyId" = t9."Id"
      ) AS t9 ON t."Id" = t9."RootId"
      """;
  }

  public class IncludeAllAttribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(RootEntity, new INavigation[]
    {
      new SimpleNavigationLeaf(RootToChildBNavigation),
        new SimpleNavigationNode(RootToChildANavigation, new INavigation[]
        {
          new SimpleNavigationNode(ChildAToChildDNavigation, new INavigation[]
          {
            new SimpleNavigationLeaf(ChildDToChildENavigation)
          }),
          new SimpleNavigationLeaf(ChildAToChildFNavigation)
        }),
        new SimpleNavigationLeaf(RootToChildCNavigation)
    });

    protected override string ExpectedTextSQLServer => """
      SELECT t.[Id], t.[Name], t2.[Id] AS [Id_2], t2.[Name] AS [Name_2], t2.[RootId] AS [RootId_2], t1.[Id_1], t1.[Name_1], t1.[RootId_1], t1.[Id_4], t1.[Name_4], t1.[ChildAId_4], t1.[Id_6], t1.[Name_6], t1.[ChildDId_6], t1.[Id_5], t1.[Name_5], t1.[ChildAId_5], t3.[Id] AS [Id_3], t3.[Name] AS [Name_3], t3.[RootId] AS [RootId_3]
      FROM (
      SELECT [Id], [Name]
      FROM [Root]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN [ChildB] AS t2 ON t.[Id] = t2.[RootId]
      LEFT JOIN (
      SELECT t.[Id] AS [Id_1], t.[Name] AS [Name_1], t.[RootId] AS [RootId_1], t4.[Id_4], t4.[Name_4], t4.[ChildAId_4], t4.[Id_6], t4.[Name_6], t4.[ChildDId_6], t5.[Id] AS [Id_5], t5.[Name] AS [Name_5], t5.[ChildAId] AS [ChildAId_5]
      FROM [ChildA] AS t
      LEFT JOIN (
      SELECT t.[Id] AS [Id_4], t.[Name] AS [Name_4], t.[ChildAId] AS [ChildAId_4], t6.[Id] AS [Id_6], t6.[Name] AS [Name_6], t6.[ChildDId] AS [ChildDId_6]
      FROM [ChildD] AS t
      LEFT JOIN [ChildE] AS t6 ON t.[Id] = t6.[ChildDId]
      ) AS t4 ON t.[Id] = t4.[ChildAId_4]
      LEFT JOIN [ChildF] AS t5 ON t.[Id] = t5.[ChildAId]
      ) AS t1 ON t.[Id] = t1.[RootId_1]
      LEFT JOIN [ChildC] AS t3 ON t.[Id] = t3.[RootId]
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT t."Id", t."Name", t2."Id" AS "Id_2", t2."Name" AS "Name_2", t2."RootId" AS "RootId_2", t1."Id_1", t1."Name_1", t1."RootId_1", t1."Id_4", t1."Name_4", t1."ChildAId_4", t1."Id_6", t1."Name_6", t1."ChildDId_6", t1."Id_5", t1."Name_5", t1."ChildAId_5", t3."Id" AS "Id_3", t3."Name" AS "Name_3", t3."RootId" AS "RootId_3"
      FROM (
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = @p0
      ) AS t
      LEFT JOIN "ChildB" AS t2 ON t."Id" = t2."RootId"
      LEFT JOIN (
      SELECT t."Id" AS "Id_1", t."Name" AS "Name_1", t."RootId" AS "RootId_1", t4."Id_4", t4."Name_4", t4."ChildAId_4", t4."Id_6", t4."Name_6", t4."ChildDId_6", t5."Id" AS "Id_5", t5."Name" AS "Name_5", t5."ChildAId" AS "ChildAId_5"
      FROM "ChildA" AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_4", t."Name" AS "Name_4", t."ChildAId" AS "ChildAId_4", t6."Id" AS "Id_6", t6."Name" AS "Name_6", t6."ChildDId" AS "ChildDId_6"
      FROM "ChildD" AS t
      LEFT JOIN "ChildE" AS t6 ON t."Id" = t6."ChildDId"
      ) AS t4 ON t."Id" = t4."ChildAId_4"
      LEFT JOIN "ChildF" AS t5 ON t."Id" = t5."ChildAId"
      ) AS t1 ON t."Id" = t1."RootId_1"
      LEFT JOIN "ChildC" AS t3 ON t."Id" = t3."RootId"
      """;

    protected override string ExpectedTextOracle => """
      SELECT t."Id", t."Name", t2."Id" AS "Id_2", t2."Name" AS "Name_2", t2."RootId" AS "RootId_2", t1."Id_1", t1."Name_1", t1."RootId_1", t1."Id_4", t1."Name_4", t1."ChildAId_4", t1."Id_6", t1."Name_6", t1."ChildDId_6", t1."Id_5", t1."Name_5", t1."ChildAId_5", t3."Id" AS "Id_3", t3."Name" AS "Name_3", t3."RootId" AS "RootId_3"
      FROM (
      SELECT "Id", "Name"
      FROM "Root"
      WHERE "Id" = :p0
      ) AS t
      LEFT JOIN "ChildB" AS t2 ON t."Id" = t2."RootId"
      LEFT JOIN (
      SELECT t."Id" AS "Id_1", t."Name" AS "Name_1", t."RootId" AS "RootId_1", t4."Id_4", t4."Name_4", t4."ChildAId_4", t4."Id_6", t4."Name_6", t4."ChildDId_6", t5."Id" AS "Id_5", t5."Name" AS "Name_5", t5."ChildAId" AS "ChildAId_5"
      FROM "ChildA" AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_4", t."Name" AS "Name_4", t."ChildAId" AS "ChildAId_4", t6."Id" AS "Id_6", t6."Name" AS "Name_6", t6."ChildDId" AS "ChildDId_6"
      FROM "ChildD" AS t
      LEFT JOIN "ChildE" AS t6 ON t."Id" = t6."ChildDId"
      ) AS t4 ON t."Id" = t4."ChildAId_4"
      LEFT JOIN "ChildF" AS t5 ON t."Id" = t5."ChildAId"
      ) AS t1 ON t."Id" = t1."RootId_1"
      LEFT JOIN "ChildC" AS t3 ON t."Id" = t3."RootId"
      """;
  }

  public class IncludeOneInverseDepth1Attribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(ChildAEntity, new INavigation[]
    {
      new SimpleNavigationLeaf(ChildAToRootNavigation)
    });

    protected override string ExpectedTextSQLServer => """
      SELECT t.[Id], t.[Name], t.[RootId], t7.[Id] AS [Id_7], t7.[Name] AS [Name_7]
      FROM (
      SELECT [Id], [Name], [RootId]
      FROM [ChildA]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN [Root] AS t7 ON t.[RootId] = t7.[Id]
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT t."Id", t."Name", t."RootId", t7."Id" AS "Id_7", t7."Name" AS "Name_7"
      FROM (
      SELECT "Id", "Name", "RootId"
      FROM "ChildA"
      WHERE "Id" = @p0
      ) AS t
      LEFT JOIN "Root" AS t7 ON t."RootId" = t7."Id"
      """;

    protected override string ExpectedTextOracle => """
      SELECT t."Id", t."Name", t."RootId", t7."Id" AS "Id_7", t7."Name" AS "Name_7"
      FROM (
      SELECT "Id", "Name", "RootId"
      FROM "ChildA"
      WHERE "Id" = :p0
      ) AS t
      LEFT JOIN "Root" AS t7 ON t."RootId" = t7."Id"
      """;
  }

  public class IncludeOneInverseDepth2Attribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(ChildAEntity, new INavigation[]
    {
      new SimpleNavigationNode(ChildAToRootNavigation, new INavigation[]
      {
        new SimpleNavigationLeaf(RootToChildBNavigation),
      })
    });

    protected override string ExpectedTextSQLServer => """
      SELECT t.[Id], t.[Name], t.[RootId], t7.[Id_7], t7.[Name_7], t7.[Id_2], t7.[Name_2], t7.[RootId_2]
      FROM (
      SELECT [Id], [Name], [RootId]
      FROM [ChildA]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN (
      SELECT t.[Id] AS [Id_7], t.[Name] AS [Name_7], t2.[Id] AS [Id_2], t2.[Name] AS [Name_2], t2.[RootId] AS [RootId_2]
      FROM [Root] AS t
      LEFT JOIN [ChildB] AS t2 ON t.[Id] = t2.[RootId]
      ) AS t7 ON t.[RootId] = t7.[Id_7]
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT t."Id", t."Name", t."RootId", t7."Id_7", t7."Name_7", t7."Id_2", t7."Name_2", t7."RootId_2"
      FROM (
      SELECT "Id", "Name", "RootId"
      FROM "ChildA"
      WHERE "Id" = @p0
      ) AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_7", t."Name" AS "Name_7", t2."Id" AS "Id_2", t2."Name" AS "Name_2", t2."RootId" AS "RootId_2"
      FROM "Root" AS t
      LEFT JOIN "ChildB" AS t2 ON t."Id" = t2."RootId"
      ) AS t7 ON t."RootId" = t7."Id_7"
      """;

    protected override string ExpectedTextOracle => """
      SELECT t."Id", t."Name", t."RootId", t7."Id_7", t7."Name_7", t7."Id_2", t7."Name_2", t7."RootId_2"
      FROM (
      SELECT "Id", "Name", "RootId"
      FROM "ChildA"
      WHERE "Id" = :p0
      ) AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_7", t."Name" AS "Name_7", t2."Id" AS "Id_2", t2."Name" AS "Name_2", t2."RootId" AS "RootId_2"
      FROM "Root" AS t
      LEFT JOIN "ChildB" AS t2 ON t."Id" = t2."RootId"
      ) AS t7 ON t."RootId" = t7."Id_7"
      """;
  }

  public class IncludeManyToManyAsLeafAfterNodeAttribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(ChildGEntity, new INavigation[]
    {
      new SimpleNavigationNode(ChildGToManyToManyNavigation, new INavigation[]
      {
        new SkipNavigationLeaf(ManyToManyToRootNavigation)
      })
    });

    protected override string ExpectedTextSQLServer => """
      SELECT t.[Id], t.[Name], t.[MTMEntityId], t11.[Id_11], t11.[Name_11], t11.[Id_10], t11.[Name_10]
      FROM (
      SELECT [Id], [Name], [MTMEntityId]
      FROM [ChildG]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN (
      SELECT t.[Id] AS [Id_11], t.[Name] AS [Name_11], t10.[Id] AS [Id_10], t10.[Name] AS [Name_10]
      FROM [ManyToMany] AS t
      LEFT JOIN (
      SELECT t10.[Id], t10.[Name], t.[ManyToManyId]
      FROM [RootManyToMany] AS t
      INNER JOIN [Root] AS t10 ON t.[RootId] = t10.[Id]
      ) AS t10 ON t.[Id] = t10.[ManyToManyId]
      ) AS t11 ON t.[MTMEntityId] = t11.[Id_11]
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT t."Id", t."Name", t."MTMEntityId", t11."Id_11", t11."Name_11", t11."Id_10", t11."Name_10"
      FROM (
      SELECT "Id", "Name", "MTMEntityId"
      FROM "ChildG"
      WHERE "Id" = @p0
      ) AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_11", t."Name" AS "Name_11", t10."Id" AS "Id_10", t10."Name" AS "Name_10"
      FROM "ManyToMany" AS t
      LEFT JOIN (
      SELECT t10."Id", t10."Name", t."ManyToManyId"
      FROM "RootManyToMany" AS t
      INNER JOIN "Root" AS t10 ON t."RootId" = t10."Id"
      ) AS t10 ON t."Id" = t10."ManyToManyId"
      ) AS t11 ON t."MTMEntityId" = t11."Id_11"
      """;

    protected override string ExpectedTextOracle => """
      SELECT t."Id", t."Name", t."MTMEntityId", t11."Id_11", t11."Name_11", t11."Id_10", t11."Name_10"
      FROM (
      SELECT "Id", "Name", "MTMEntityId"
      FROM "ChildG"
      WHERE "Id" = :p0
      ) AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_11", t."Name" AS "Name_11", t10."Id" AS "Id_10", t10."Name" AS "Name_10"
      FROM "ManyToMany" AS t
      LEFT JOIN (
      SELECT t10."Id", t10."Name", t."ManyToManyId"
      FROM "RootManyToMany" AS t
      INNER JOIN "Root" AS t10 ON t."RootId" = t10."Id"
      ) AS t10 ON t."Id" = t10."ManyToManyId"
      ) AS t11 ON t."MTMEntityId" = t11."Id_11"
      """;
  }

  public class IncludeManyToManyAsNodeDepth1Attribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(ChildGEntity, new INavigation[]
    {
      new SimpleNavigationNode(ChildGToManyToManyNavigation, new INavigation[]
      {
        new SkipNavigationNode(ManyToManyToRootNavigation, new INavigation[]
        {
          new SimpleNavigationLeaf(RootToChildANavigation)
        })
      })
    });

    protected override string ExpectedTextSQLServer => """
      SELECT t.[Id], t.[Name], t.[MTMEntityId], t11.[Id_11], t11.[Name_11], t11.[Id_10], t11.[Name_10], t11.[Id_1], t11.[Name_1], t11.[RootId_1]
      FROM (
      SELECT [Id], [Name], [MTMEntityId]
      FROM [ChildG]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN (
      SELECT t.[Id] AS [Id_11], t.[Name] AS [Name_11], t10.[Id_10], t10.[Name_10], t10.[Id_1], t10.[Name_1], t10.[RootId_1]
      FROM [ManyToMany] AS t
      LEFT JOIN (
      SELECT t10.[Id_10], t10.[Name_10], t10.[Id_1], t10.[Name_1], t10.[RootId_1], t.[ManyToManyId]
      FROM [RootManyToMany] AS t
      INNER JOIN (
      SELECT t.[Id] AS [Id_10], t.[Name] AS [Name_10], t1.[Id] AS [Id_1], t1.[Name] AS [Name_1], t1.[RootId] AS [RootId_1]
      FROM [Root] AS t
      LEFT JOIN [ChildA] AS t1 ON t.[Id] = t1.[RootId]
      ) AS t10 ON t.[RootId] = t10.[Id_10]
      ) AS t10 ON t.[Id] = t10.[ManyToManyId]
      ) AS t11 ON t.[MTMEntityId] = t11.[Id_11]
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT t."Id", t."Name", t."MTMEntityId", t11."Id_11", t11."Name_11", t11."Id_10", t11."Name_10", t11."Id_1", t11."Name_1", t11."RootId_1"
      FROM (
      SELECT "Id", "Name", "MTMEntityId"
      FROM "ChildG"
      WHERE "Id" = @p0
      ) AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_11", t."Name" AS "Name_11", t10."Id_10", t10."Name_10", t10."Id_1", t10."Name_1", t10."RootId_1"
      FROM "ManyToMany" AS t
      LEFT JOIN (
      SELECT t10."Id_10", t10."Name_10", t10."Id_1", t10."Name_1", t10."RootId_1", t."ManyToManyId"
      FROM "RootManyToMany" AS t
      INNER JOIN (
      SELECT t."Id" AS "Id_10", t."Name" AS "Name_10", t1."Id" AS "Id_1", t1."Name" AS "Name_1", t1."RootId" AS "RootId_1"
      FROM "Root" AS t
      LEFT JOIN "ChildA" AS t1 ON t."Id" = t1."RootId"
      ) AS t10 ON t."RootId" = t10."Id_10"
      ) AS t10 ON t."Id" = t10."ManyToManyId"
      ) AS t11 ON t."MTMEntityId" = t11."Id_11"
      """;

    protected override string ExpectedTextOracle => """
      SELECT t."Id", t."Name", t."MTMEntityId", t11."Id_11", t11."Name_11", t11."Id_10", t11."Name_10", t11."Id_1", t11."Name_1", t11."RootId_1"
      FROM (
      SELECT "Id", "Name", "MTMEntityId"
      FROM "ChildG"
      WHERE "Id" = :p0
      ) AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_11", t."Name" AS "Name_11", t10."Id_10", t10."Name_10", t10."Id_1", t10."Name_1", t10."RootId_1"
      FROM "ManyToMany" AS t
      LEFT JOIN (
      SELECT t10."Id_10", t10."Name_10", t10."Id_1", t10."Name_1", t10."RootId_1", t."ManyToManyId"
      FROM "RootManyToMany" AS t
      INNER JOIN (
      SELECT t."Id" AS "Id_10", t."Name" AS "Name_10", t1."Id" AS "Id_1", t1."Name" AS "Name_1", t1."RootId" AS "RootId_1"
      FROM "Root" AS t
      LEFT JOIN "ChildA" AS t1 ON t."Id" = t1."RootId"
      ) AS t10 ON t."RootId" = t10."Id_10"
      ) AS t10 ON t."Id" = t10."ManyToManyId"
      ) AS t11 ON t."MTMEntityId" = t11."Id_11"
      """;
  }

  public class IncludeManyToManyAsNodeDepth2Attribute : SelectTextDataAttribute
  {
    private protected override NavigationSpec Spec => new(ChildGEntity, new INavigation[]
    {
      new SimpleNavigationNode(ChildGToManyToManyNavigation, new INavigation[]
      {
        new SkipNavigationNode(ManyToManyToRootNavigation, new INavigation[]
        {
          new SimpleNavigationNode(RootToChildANavigation, new INavigation[]
          {
            new SimpleNavigationLeaf(ChildAToChildDNavigation)
          })
        })
      })
    });

    protected override string ExpectedTextSQLServer => """
      SELECT t.[Id], t.[Name], t.[MTMEntityId], t11.[Id_11], t11.[Name_11], t11.[Id_10], t11.[Name_10], t11.[Id_1], t11.[Name_1], t11.[RootId_1], t11.[Id_4], t11.[Name_4], t11.[ChildAId_4]
      FROM (
      SELECT [Id], [Name], [MTMEntityId]
      FROM [ChildG]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN (
      SELECT t.[Id] AS [Id_11], t.[Name] AS [Name_11], t10.[Id_10], t10.[Name_10], t10.[Id_1], t10.[Name_1], t10.[RootId_1], t10.[Id_4], t10.[Name_4], t10.[ChildAId_4]
      FROM [ManyToMany] AS t
      LEFT JOIN (
      SELECT t10.[Id_10], t10.[Name_10], t10.[Id_1], t10.[Name_1], t10.[RootId_1], t10.[Id_4], t10.[Name_4], t10.[ChildAId_4], t.[ManyToManyId]
      FROM [RootManyToMany] AS t
      INNER JOIN (
      SELECT t.[Id] AS [Id_10], t.[Name] AS [Name_10], t1.[Id_1], t1.[Name_1], t1.[RootId_1], t1.[Id_4], t1.[Name_4], t1.[ChildAId_4]
      FROM [Root] AS t
      LEFT JOIN (
      SELECT t.[Id] AS [Id_1], t.[Name] AS [Name_1], t.[RootId] AS [RootId_1], t4.[Id] AS [Id_4], t4.[Name] AS [Name_4], t4.[ChildAId] AS [ChildAId_4]
      FROM [ChildA] AS t
      LEFT JOIN [ChildD] AS t4 ON t.[Id] = t4.[ChildAId]
      ) AS t1 ON t.[Id] = t1.[RootId_1]
      ) AS t10 ON t.[RootId] = t10.[Id_10]
      ) AS t10 ON t.[Id] = t10.[ManyToManyId]
      ) AS t11 ON t.[MTMEntityId] = t11.[Id_11]
      """;

    protected override string ExpectedTextPostgreSQL => """
      SELECT t."Id", t."Name", t."MTMEntityId", t11."Id_11", t11."Name_11", t11."Id_10", t11."Name_10", t11."Id_1", t11."Name_1", t11."RootId_1", t11."Id_4", t11."Name_4", t11."ChildAId_4"
      FROM (
      SELECT "Id", "Name", "MTMEntityId"
      FROM "ChildG"
      WHERE "Id" = @p0
      ) AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_11", t."Name" AS "Name_11", t10."Id_10", t10."Name_10", t10."Id_1", t10."Name_1", t10."RootId_1", t10."Id_4", t10."Name_4", t10."ChildAId_4"
      FROM "ManyToMany" AS t
      LEFT JOIN (
      SELECT t10."Id_10", t10."Name_10", t10."Id_1", t10."Name_1", t10."RootId_1", t10."Id_4", t10."Name_4", t10."ChildAId_4", t."ManyToManyId"
      FROM "RootManyToMany" AS t
      INNER JOIN (
      SELECT t."Id" AS "Id_10", t."Name" AS "Name_10", t1."Id_1", t1."Name_1", t1."RootId_1", t1."Id_4", t1."Name_4", t1."ChildAId_4"
      FROM "Root" AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_1", t."Name" AS "Name_1", t."RootId" AS "RootId_1", t4."Id" AS "Id_4", t4."Name" AS "Name_4", t4."ChildAId" AS "ChildAId_4"
      FROM "ChildA" AS t
      LEFT JOIN "ChildD" AS t4 ON t."Id" = t4."ChildAId"
      ) AS t1 ON t."Id" = t1."RootId_1"
      ) AS t10 ON t."RootId" = t10."Id_10"
      ) AS t10 ON t."Id" = t10."ManyToManyId"
      ) AS t11 ON t."MTMEntityId" = t11."Id_11"
      """;

    protected override string ExpectedTextOracle => """
      SELECT t."Id", t."Name", t."MTMEntityId", t11."Id_11", t11."Name_11", t11."Id_10", t11."Name_10", t11."Id_1", t11."Name_1", t11."RootId_1", t11."Id_4", t11."Name_4", t11."ChildAId_4"
      FROM (
      SELECT "Id", "Name", "MTMEntityId"
      FROM "ChildG"
      WHERE "Id" = :p0
      ) AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_11", t."Name" AS "Name_11", t10."Id_10", t10."Name_10", t10."Id_1", t10."Name_1", t10."RootId_1", t10."Id_4", t10."Name_4", t10."ChildAId_4"
      FROM "ManyToMany" AS t
      LEFT JOIN (
      SELECT t10."Id_10", t10."Name_10", t10."Id_1", t10."Name_1", t10."RootId_1", t10."Id_4", t10."Name_4", t10."ChildAId_4", t."ManyToManyId"
      FROM "RootManyToMany" AS t
      INNER JOIN (
      SELECT t."Id" AS "Id_10", t."Name" AS "Name_10", t1."Id_1", t1."Name_1", t1."RootId_1", t1."Id_4", t1."Name_4", t1."ChildAId_4"
      FROM "Root" AS t
      LEFT JOIN (
      SELECT t."Id" AS "Id_1", t."Name" AS "Name_1", t."RootId" AS "RootId_1", t4."Id" AS "Id_4", t4."Name" AS "Name_4", t4."ChildAId" AS "ChildAId_4"
      FROM "ChildA" AS t
      LEFT JOIN "ChildD" AS t4 ON t."Id" = t4."ChildAId"
      ) AS t1 ON t."Id" = t1."RootId_1"
      ) AS t10 ON t."RootId" = t10."Id_10"
      ) AS t10 ON t."Id" = t10."ManyToManyId"
      ) AS t11 ON t."MTMEntityId" = t11."Id_11"
      """;
  }

  private record SimpleNavigationLeaf(IAccessibleSimpleNavigationModel Model) : ISimpleNavigationLeaf
  {
    public IEntityModel Target => Model.To;

    public IReadOnlyCollection<INavigation> Children => Array.Empty<INavigation>();

    IAccessibleNavigationModel INavigation.Model => Model;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
      where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitSimpleLeaf(this);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }

  private record SkipNavigationLeaf(IAccessibleSkipNavigationModel Model) : ISkipNavigationLeaf
  {
    public IEntityModel Target => Model.To;

    public IReadOnlyCollection<INavigation> Children => Array.Empty<INavigation>();

    IAccessibleNavigationModel INavigation.Model => Model;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
      where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitSkipLeaf(this);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }

  private record SimpleNavigationNode(IAccessibleSimpleNavigationModel Model, IReadOnlyCollection<INavigation> Children) : ISimpleNavigationNode
  {
    public IEntityModel Target => Model.To;

    IAccessibleNavigationModel INavigation.Model => Model;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor) where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitSimpleNode(this);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }

  private record SkipNavigationNode(IAccessibleSkipNavigationModel Model, IReadOnlyCollection<INavigation> Children) : ISkipNavigationNode
  {
    public IEntityModel Target => Model.To;

    IAccessibleSkipNavigationModel ISkipNavigationNode.Model => Model;

    IAccessibleNavigationModel INavigation.Model => Model;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor) where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitSkipNode(this);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }
}
