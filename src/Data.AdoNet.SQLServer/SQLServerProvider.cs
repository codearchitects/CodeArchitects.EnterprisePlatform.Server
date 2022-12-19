using CodeArchitects.Platform.Data.AdoNet.SQLServer.Command;

namespace CodeArchitects.Platform.Data.AdoNet.SQLServer;

internal class SQLServerProvider : DatabaseProvider
{
  private protected override Type DataContextType => typeof(SQLServerDataContext);

  private protected override Type SyntaxProviderType => typeof(SQLServerSyntaxProvider);
}
