namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTOOwned;

public class Secondary : Entity
{
  public string? Name { get; set; }
  public Primary? Primary { get; set; }
}
