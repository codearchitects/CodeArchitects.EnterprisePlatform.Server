namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTOAssociated
{
  public class Primary : Entity
  {
    public string? Name { get; set; }
    public Secondary? Secondary { get; set; }
  }
}
