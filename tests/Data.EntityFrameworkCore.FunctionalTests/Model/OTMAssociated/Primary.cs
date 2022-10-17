namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTMAssociated;

public class Primary : Entity
{
  public string? Name { get; set; }
  public ICollection<Secondary>? Secondaries { get; set; }
}
