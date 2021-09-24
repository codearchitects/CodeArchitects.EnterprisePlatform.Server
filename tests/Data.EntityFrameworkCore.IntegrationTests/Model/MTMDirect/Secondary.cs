using System.Collections.Generic;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTMDirect
{
  public class Secondary : Entity
  {
    public string? Name { get; set; }
    public ICollection<Primary>? Primaries { get; set; }
  }
}
