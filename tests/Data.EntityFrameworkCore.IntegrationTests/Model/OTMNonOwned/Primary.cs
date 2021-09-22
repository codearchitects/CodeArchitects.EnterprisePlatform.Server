using System.Collections.Generic;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTMNonOwned
{
  public class Primary : Entity
  {
    public string? Name { get; set; }
    public ICollection<Secondary>? Secondaries { get; set; }
  }
}
