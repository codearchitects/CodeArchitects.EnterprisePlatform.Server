using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class OTOAssociationBuilder<TFrom, TTo> : AssociationBuilder, IOTOAssociationBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  private readonly List<Name> _foreignKeyNames;
  private MemberInfo? _navigation;
  private MemberInfo? _inverseNavigation;

  public OTOAssociationBuilder()
  {
    _foreignKeyNames = new();
  }

  public override Association Build(AssociationKind kind)
  {
    if (_navigation is null && _inverseNavigation is null)
      throw new ModelConfigurationException($"No navigation was specified for association '{typeof(TFrom).Name}' -> '{typeof(TTo).Name}'.");

    return new OTOAssociation(kind, typeof(TFrom), typeof(TTo), _navigation, _inverseNavigation, _foreignKeyNames);
  }

  public IOTOAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression)
  {
    if (!TryGetPropertyOrField(expression.Body, out MemberInfo? member))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    _inverseNavigation = member;

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName)
  {
    _inverseNavigation = GetMember(typeof(TTo), navigationName);

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, TTo?>> expression)
  {
    if (!TryGetPropertyOrField(expression.Body, out MemberInfo? member))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    _navigation = member;

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> Navigation(string navigationName)
  {
    _navigation = GetMember(typeof(TFrom), navigationName);

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> UsingForeignKey<TForeignKey>(Expression<Func<TTo, TForeignKey>> expression)
  {
    _foreignKeyNames.Clear();
    _foreignKeyNames.AddRange(GetKeyNames(expression.Body));

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> UsingForeignKey(params string[] keyNames)
  {
    _foreignKeyNames.Clear();

    foreach (string keyName in keyNames)
    {
      _ = GetMemberAndType(typeof(TFrom), keyName);

      _foreignKeyNames.Add(keyName);
    }

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> UsingForeignKey(params Name[] keyNames)
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
