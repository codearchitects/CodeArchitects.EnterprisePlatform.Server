using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract class NavigationModelBuilder : BuilderBase, IEquatable<NavigationModelBuilder>
{
  public abstract IReadOnlyCollection<Name> ForeignKeyNames { get; }
  protected abstract string FromEntityName { get; }
  protected abstract string ToEntityName { get; }
  protected abstract MemberInfo? DirectNavigationMember { get; }
  protected abstract MemberInfo? InverseNavigationMember { get; }

  public abstract (NavigationModel Direct, NavigationModel Inverse) Build(DataModel dataModel);

  public bool Equals([AllowNull] NavigationModelBuilder other)
  {
    if (other is null)
      return false;

    return
      FromEntityName == other.FromEntityName &&
      ToEntityName == other.ToEntityName &&
      DirectNavigationMember == other.DirectNavigationMember &&
      InverseNavigationMember == other.InverseNavigationMember;
  }

  public override bool Equals(object? obj)
  {
    return Equals(obj as NavigationModelBuilder);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(FromEntityName, ToEntityName);
  }
}
