using System;

namespace CodeArchitects.Platform.Data
{
  public interface IEntity<out TKey> : IEntity
    where TKey : notnull, IEquatable<TKey>
  {
    new TKey Id { get; }
  }
}
