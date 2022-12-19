using CodeArchitects.Platform.Data.AdoNet.PostgreSQL.Command;

namespace CodeArchitects.Platform.Data.AdoNet.PostgreSQL;

internal class PostgreSQLProvider : DatabaseProvider
{
  private protected override Type DataContextType => typeof(PostgreSQLDataContext);

  private protected override Type SyntaxProviderType => typeof(PostgreSQLSyntaxProvider);
}
