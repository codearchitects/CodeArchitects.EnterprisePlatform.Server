using System;

namespace CodeArchitects.Platform.Data;

// TODO: Use the ORM's object model instead
public interface IEntity<out TKey> : IEntity
  where TKey : notnull, IEquatable<TKey>
{
  new TKey Id { get; }
}
