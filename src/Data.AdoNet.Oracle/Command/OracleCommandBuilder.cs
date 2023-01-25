using CodeArchitects.Platform.Data.AdoNet.Command;
using Oracle.ManagedDataAccess.Client;

namespace CodeArchitects.Platform.Data.AdoNet.Oracle.Command;

internal class OracleCommandBuilder : CommandBuilder<OracleCommand>
{
  public OracleCommandBuilder(ISqlTextBuilder sqlBuilder)
    : base(sqlBuilder)
  {
  }

  protected override void CreateParameter(OracleCommand command, string name, object? value)
  {
    OracleParameter parameter = command.CreateParameter();

    if (value is null)
    {
      value = DBNull.Value;
    }
    else
    {
      if (value.GetType() == typeof(Guid))
      {
        parameter.OracleDbType = OracleDbType.Raw;
      }
    }

    parameter.ParameterName = name;
    parameter.Value = value ?? DBNull.Value;
    command.Parameters.Add(parameter);
  }
}
