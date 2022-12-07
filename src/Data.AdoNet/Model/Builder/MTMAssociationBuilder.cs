using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class MTMAssociationBuilder<TFrom, TTo> : AssociationBuilder, IMTMAssociationBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  private MemberInfo? _navigation;
  private MemberInfo? _inverseNavigation;
  private string? _tableName;
  private IReadOnlyCollection<string>? _foreignKeyNames;

  public override Association Build(AssociationKind kind)
  {
    Debug.Assert(kind is AssociationKind.Composition);

    if (_navigation is null && _inverseNavigation is null)
      throw new ModelConfigurationException($"No navigation was specified for association '{typeof(TFrom).Name}' -> '{typeof(TTo).Name}'.");

    return new MTMAssociation(typeof(TFrom), typeof(TTo), _navigation, _inverseNavigation, _tableName ?? $"{typeof(TFrom)}{typeof(TTo)}", _foreignKeyNames ?? Array.Empty<string>());
  }

  public IMTMAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression)
  {
    if (!TryGetPropertyOrField(expression.Body, out MemberInfo? member))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    _inverseNavigation = member;

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName)
  {
    _inverseNavigation = GetMember(typeof(TTo), navigationName);

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, IEnumerable<TTo>?>> expression)
  {
    if (!TryGetPropertyOrField(expression.Body, out MemberInfo? member))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    _navigation = member;

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> Navigation(string navigationName)
  {
    _navigation = GetMember(typeof(TFrom), navigationName);

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> UsingForeignKeys(params string[] keyNames)
  {
    _foreignKeyNames = keyNames;

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> UsingJoinTable(string tableName)
  {
    _tableName = tableName;

    return this;
  }
}
