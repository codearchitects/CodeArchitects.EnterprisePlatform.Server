using CodeArchitects.Platform.Data.AdoNet.Oracle.Command;

namespace CodeArchitects.Platform.Data.AdoNet.Oracle;

internal class OracleProvider : DatabaseProvider
{
  private protected override Type DataContextType => typeof(OracleDataContext);

  private protected override Type SyntaxProviderType => typeof(OracleSyntaxProvider);
}
