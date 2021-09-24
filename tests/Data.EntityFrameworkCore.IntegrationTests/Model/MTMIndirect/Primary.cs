using System.Collections.Generic;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTMIndirect
{
  public class Primary : Entity
  {
    public string? Name { get; set; }
    public ICollection<PrimarySecondary>? Secondaries { get; set; }
  }
}
