namespace CodeArchitects.Platform.GraphQL.Buffers;

internal interface IBufferProvider
{
  BufferOwner GetBuffer();
}
