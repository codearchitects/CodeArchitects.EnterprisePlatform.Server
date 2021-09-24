using System.Collections.Generic;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTMDirect
{
  public class Primary : Entity
  {
    public string? Name { get; set; }
    public ICollection<Secondary>? Secondaries { get; set; }
  }
}
