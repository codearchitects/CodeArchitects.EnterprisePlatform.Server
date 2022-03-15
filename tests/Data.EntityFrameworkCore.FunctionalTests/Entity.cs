using System;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class Entity : IEntity<Guid>
{
  public Guid Id { get; set; }

  object IEntity.Id => Id;
}
