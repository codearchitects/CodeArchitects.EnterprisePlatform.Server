namespace EFCoreSample.Domain.Model;

public class Cart
{
  public Guid Id { get; set; }
  public List<Order> Orders { get; set; }
}
