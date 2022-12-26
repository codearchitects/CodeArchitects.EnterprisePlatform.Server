using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract class EntityModelBuilder : BuilderBase
{
  public abstract IReadOnlyCollection<MemberInfo> PrimaryKeyMembers { get; }

  public abstract Type EntityType { get; }

  public abstract EntityModel Build(int id);

  public abstract void AddSimpleNavigation(SimpleNavigationModel navigation, IEnumerable<Name> foreignKeyNames);

  public abstract void AddSkipNavigation(SkipNavigationModel navigation, IEnumerable<Name> columnNames);

  public abstract void AddPrimaryKeyColumns();

  public abstract void AddAccessibleColumns();

  public abstract void AddHiddenColumns();

  public abstract void AddJoinTableColumns();
}
