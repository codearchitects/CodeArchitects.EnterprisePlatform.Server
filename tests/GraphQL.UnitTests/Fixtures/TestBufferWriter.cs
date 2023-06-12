using System.Buffers;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.Fixtures;

internal class TestBufferWriter : IBufferWriter<byte>, IDisposable
{
  private readonly StringBuilder _sb;
  private readonly MemoryStream _ms;

  public TestBufferWriter()
  {
    _sb = new();
    _ms = new();
  }

  public void Advance(int count)
  {
    string @string = Encoding.UTF8.GetString(_ms.GetBuffer().AsSpan(0, count));
    _sb.Append(@string);
  }

  public Span<byte> GetSpan(int sizeHint = 0)
  {
    if (sizeHint > _ms.Capacity)
    {
      _ms.Capacity = sizeHint;
    }
    _ms.Position = 0;
    return _ms.GetBuffer();
  }

  public void Dispose()
  {
    ((IDisposable)_ms).Dispose();
  }

  public override string ToString() => _sb.ToString();

  public Memory<byte> GetMemory(int sizeHint = 0) => throw new NotImplementedException();
}
