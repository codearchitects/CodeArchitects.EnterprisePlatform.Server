using Microsoft.Extensions.ObjectPool;
using System.Buffers;

namespace CodeArchitects.Platform.GraphQL.Buffers;

internal class BufferProvider : IBufferProvider
{
  private readonly DefaultObjectPool<ArrayBufferWriter<byte>> _pool;

  public BufferProvider(DefaultObjectPool<ArrayBufferWriter<byte>> pool)
  {
    _pool = pool;
  }

  public BufferOwner GetBuffer()
  {
    ArrayBufferWriter<byte> writer = _pool.Get();
    return new BufferOwner(_pool, writer);
  }

  public static BufferProvider Create()
  {
    DefaultObjectPool<ArrayBufferWriter<byte>> pool = new(PooledBufferPolicy.Instance);
    return new(pool);
  }
}
