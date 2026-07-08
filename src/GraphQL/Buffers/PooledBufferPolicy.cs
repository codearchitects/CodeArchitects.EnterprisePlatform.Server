using Microsoft.Extensions.ObjectPool;
using System.Buffers;

namespace CodeArchitects.Platform.GraphQL.Buffers;

internal class PooledBufferPolicy : IPooledObjectPolicy<ArrayBufferWriter<byte>>
{
  private const int s_initialCapacity = 256;
  private const int s_maximumRetainedCapacity = 2 * 1024;

  public static readonly PooledBufferPolicy Instance = new();

  private PooledBufferPolicy()
  {
  }

  public ArrayBufferWriter<byte> Create()
  {
    return new(s_initialCapacity);
  }

  public bool Return(ArrayBufferWriter<byte> obj)
  {
    if (obj.Capacity > s_maximumRetainedCapacity)
      return false;

    obj.Clear();
    return true;
  }
}
