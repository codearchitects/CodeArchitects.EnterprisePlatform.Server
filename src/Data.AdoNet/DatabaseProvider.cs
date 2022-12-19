using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet.Command;
using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

public abstract class DatabaseProvider
{
  private protected DatabaseProvider() { }

  private protected abstract Type DbConnectionType { get; }

  private protected abstract Type DbCommandType { get; }

  internal abstract Type SyntaxProviderType { get; }

  internal IConnectionFactory<DbConnection>? DelegateConnectionFactory { get; private protected set; }

  internal Type? ConnectionFactoryType { get; private protected set; }

  internal virtual Type DataContextType => MakeGenericType(typeof(DataContext<,>));

  internal virtual Type CommandBuilderType => MakeGenericType(typeof(CommandBuilder<>));

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
