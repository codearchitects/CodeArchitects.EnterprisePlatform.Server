namespace EFCoreSample.Domain.Model;

public class Order
{
  public Guid Id { get; set; }
  public string? ShippingAddress { get; set; }
  public List<Product>? Products { get; set; }
}
