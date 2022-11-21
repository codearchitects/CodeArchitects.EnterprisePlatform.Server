using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class CartItem
{
  public int Index { get; set; }
  public Guid CartId { get; set; }
  public string? Name { get; set; }
  public Cart? Cart { get; set; } // OTM aggregation, on dependent
  public List<Product>? Products { get; set; } // MTM
  public ShippingAddress? ShippingAddress { get; set; } // OTM, on dependent

  private static readonly Faker<CartItem> s_faker = new Faker<CartItem>()
    .RuleFor(cart => cart.Index, faker => faker.IndexGlobal)
    .RuleFor(cart => cart.Name, faker => faker.Lorem.Word());

  public static CartItem One(Guid cartId)
  {
    var item = s_faker.Generate();
    item.CartId = cartId;
    return item;
  }

  public static List<CartItem> Many(int count, Guid cartId)
  {
    var list = s_faker.Generate(count);
    list.ForEach(item => item.CartId = cartId);
    return list;
  }
}
