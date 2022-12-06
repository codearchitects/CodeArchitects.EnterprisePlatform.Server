using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class OTMAssociationBuilder<TFrom, TTo> : AssociationBuilder, IOTMAssociationBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  private readonly List<Name> _foreignKeyNames;
  private MemberInfo? _navigation;
  private MemberInfo? _inverseNavigation;

  public OTMAssociationBuilder()
  {
    _foreignKeyNames = new();
  }

  public override Association Build(AssociationKind kind)
  {
    if (_navigation is null && _inverseNavigation is null)
      throw new ModelConfigurationException($"No navigation was specified for association '{typeof(TFrom).Name}' -> '{typeof(TTo).Name}'.");

    return new OTMAssociation(kind, typeof(TFrom), typeof(TTo), _navigation, _inverseNavigation, _foreignKeyNames);
  }

  public IOTMAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression)
  {
    if (!TryGetPropertyOrField(expression.Body, out MemberInfo? member))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    _inverseNavigation = member;

    return this;
  }

  public IOTMAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName)
  {
    (MemberInfo member, _) = GetMemberAndType(typeof(TTo), navigationName);

    _inverseNavigation = member;

    return this;
  }

  public IOTMAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, IEnumerable<TTo>?>> expression)
  {
    if (!TryGetPropertyOrField(expression.Body, out MemberInfo? member))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    _navigation = member;

    return this;
  }

  public IOTMAssociationBuilder<TFrom, TTo> Navigation(string navigationName)
  {
    (MemberInfo member, _) = GetMemberAndType(typeof(TFrom), navigationName);
    _navigation = member;

    return this;
  }

  public IOTMAssociationBuilder<TFrom, TTo> UsingForeignKey<TForeignKey>(Expression<Func<TTo, TForeignKey>> expression)
  {
    _foreignKeyNames.Clear();
    _foreignKeyNames.AddRange(GetKeyNames(expression.Body));

    return this;
  }

  public IOTMAssociationBuilder<TFrom, TTo> UsingForeignKey(params string[] keyNames)
  {
    _foreignKeyNames.Clear();

    foreach (string keyName in keyNames)
    {
      _ = GetMemberAndType(typeof(TFrom), keyName);

      _foreignKeyNames.Add(keyName);
    }

    return this;
  }

  public IOTMAssociationBuilder<TFrom, TTo> UsingForeignKey(params Name[] keyNames)
  {
    _foreignKeyNames.Clear();

    foreach (Name keyName in keyNames)
    {
      if (!keyName.IsColumnName)
      {
        _ = GetMemberAndType(typeof(TFrom), keyName.Value);
      }

      _foreignKeyNames.Add(keyName);
    }

    return this;
  }
}
