using Microsoft.Extensions.ObjectPool;
using System.Buffers;

namespace CodeArchitects.Platform.GraphQL.Buffers;

internal readonly ref struct BufferOwner
{
  private readonly ObjectPool<ArrayBufferWriter<byte>> _pool;

  public BufferOwner(ObjectPool<ArrayBufferWriter<byte>> pool, ArrayBufferWriter<byte> writer)
  {
    _pool = pool;
    Writer = writer;
  }

  public ArrayBufferWriter<byte> Writer { get; }

  public void Dispose()
  {
    _pool.Return(Writer);
  }
}
