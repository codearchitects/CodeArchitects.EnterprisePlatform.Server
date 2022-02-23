using System.Collections.Generic;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTMIndirect;

public class Secondary : Entity
{
  public string? Name { get; set; }
  public ICollection<PrimarySecondary>? Primaries { get; set; }
}
