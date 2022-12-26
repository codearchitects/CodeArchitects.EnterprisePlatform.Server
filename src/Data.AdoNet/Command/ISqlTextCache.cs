using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ISqlTextCache
{
  string GetOrAddFindText(INavigationRoot root, SqlTextBuilder sqlBuilder, Func<INavigationRoot, SqlTextBuilder, string> queryCompiler);

  string GetOrAddInsertText(IEntityModel entityModel, SqlTextBuilder sqlBuilder, Func<IEntityModel, SqlTextBuilder, string> queryCompiler);

  string GetOrAddUpdateText(IEntityModel entityModel, SqlTextBuilder sqlBuilder, Func<IEntityModel, SqlTextBuilder, string> queryCompiler);

  string GetOrAddUpsertText(IEntityModel entityModel, SqlTextBuilder sqlBuilder, Func<IEntityModel, SqlTextBuilder, string> queryCompiler);

  string GetOrAddRemoveText(IEntityModel entityModel, SqlTextBuilder sqlBuilder, Func<IEntityModel, SqlTextBuilder, string> queryCompiler);
}
