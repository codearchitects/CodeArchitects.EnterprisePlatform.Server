using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet.Command;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// Represents a database provider.
/// </summary>
public abstract class DatabaseProvider
{
  private protected DatabaseProvider() { }

  private protected abstract Type DbConnectionType { get; }

  private protected abstract Type DbCommandType { get; }

  internal abstract Type DataContextType { get; }

  internal IConnectionFactory<DbConnection>? DelegateConnectionFactory { get; private protected set; }

  internal Type? ConnectionFactoryType { get; private protected set; }

  private protected abstract ISyntaxProvider CreateSyntaxProvider();

  private protected abstract object CreateCommandBuilderCore(ISqlTextBuilder sqlBuilder);

  public void ApplySeed(IServiceProvider services)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    if (services.GetService(typeof(DataSeed)) is not DataSeed seed)
      return;

    ApplySeed(services, seed);
  }

  internal abstract void ApplySeed(IServiceProvider services, DataSeed seed);

  internal object CreateCommandBuilder(IMemoryCache cache)
  {
    SqlTextCache sqlCache = new(cache);
    ISyntaxProvider syntaxProvider = CreateSyntaxProvider();
    SqlTextBuilder sqlBuilder = new(sqlCache, syntaxProvider);

    return CreateCommandBuilderCore(sqlBuilder);
  }

  internal Type MakeGenericType(Type genericTypeDefinition)
  {
    Type[] typeArguments = genericTypeDefinition
      .GetGenericArguments()
      .Map(argument =>
        IsDbConnection(argument) ? DbConnectionType :
        IsDbCommand(argument) ? DbCommandType :
        throw new ArgumentException($"Could not resolve generic type parameter for type '{genericTypeDefinition.Name}'", nameof(genericTypeDefinition)));

    return genericTypeDefinition.MakeGenericType(typeArguments);

    static bool IsDbConnection(Type typeArgument)
    {
      return IsConstraint(typeArgument, typeof(IDbConnection));
    }

    static bool IsDbCommand(Type typeArgument)
    {
      return IsConstraint(typeArgument, typeof(IDbCommand));
    }

    static bool IsConstraint(Type typeArgument, Type constraintType)
    {
      return typeArgument
        .GetGenericParameterConstraints()
        .Any(constraint => constraintType.IsAssignableFrom(constraint));
    }
  }

  protected internal void UseConnectionFactoryCore(Type connectionFactoryType)
  {
    Type interfaceType = MakeGenericType(typeof(IConnectionFactory<>));
    if (!interfaceType.IsAssignableFrom(connectionFactoryType))
      throw new ArgumentException($"The factory must implement '{interfaceType}'.", nameof(connectionFactoryType));

    ConnectionFactoryType = connectionFactoryType;
  }
}
