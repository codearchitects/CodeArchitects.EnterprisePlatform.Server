using System.Collections;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Fixtures;

internal class FakeDbDataReader : DbDataReader
{
  private readonly object?[,] _table;
  private int _index;

  public FakeDbDataReader(object?[,] table)
  {
    _table = table;
    _index = -1;
  }

  public override bool GetBoolean(int i)
  {
    return (bool)GetValue(i);
  }

  public override byte GetByte(int i)
  {
    return (byte)GetValue(i);
  }

  public override char GetChar(int i)
  {
    return (char)GetValue(i);
  }

  public override DateTime GetDateTime(int i)
  {
    return (DateTime)GetValue(i);
  }

  public override decimal GetDecimal(int i)
  {
    return (decimal)GetValue(i);
  }

  public override double GetDouble(int i)
  {
    return (double)GetValue(i);
  }

  public override float GetFloat(int i)
  {
    return (float)GetValue(i);
  }

  public override Guid GetGuid(int i)
  {
    return (Guid)GetValue(i);
  }

  public override short GetInt16(int i)
  {
    return (short)GetValue(i);
  }

  public override int GetInt32(int i)
  {
    return (int)GetValue(i);
  }

  public override long GetInt64(int i)
  {
    return (long)GetValue(i);
  }

  public override string GetString(int i)
  {
    return (string)GetValue(i);
  }

  public override object GetValue(int i)
  {
    return _table[_index, i] ?? throw new InvalidOperationException($"Value [{_index}, {i}] was null.");
  }

  public override bool IsDBNull(int i)
  {
    return _table[_index, i] is null;
  }

  public override bool Read()
  {
    _index++;
    return _index != _table.GetLength(0);
  }

  #region Not implemented

  public override int Depth => throw new NotImplementedException();

  public override int FieldCount => throw new NotImplementedException();

  public override bool HasRows => throw new NotImplementedException();

  public override bool IsClosed => throw new NotImplementedException();

  public override int RecordsAffected => throw new NotImplementedException();

  public override object this[string name] => throw new NotImplementedException();

  public override object this[int ordinal] => throw new NotImplementedException();

  public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
  {
    throw new NotImplementedException();
  }

  public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
  {
    throw new NotImplementedException();
  }

  public override string GetDataTypeName(int ordinal)
  {
    throw new NotImplementedException();
  }

  public override IEnumerator GetEnumerator()
  {
    throw new NotImplementedException();
  }

  public override Type GetFieldType(int ordinal)
  {
    throw new NotImplementedException();
  }

  public override string GetName(int ordinal)
  {
    throw new NotImplementedException();
  }

  public override int GetOrdinal(string name)
  {
    throw new NotImplementedException();
  }

  public override int GetValues(object[] values)
  {
    throw new NotImplementedException();
  }

  public override bool NextResult()
  {
    throw new NotImplementedException();
  }

  #endregion
}
