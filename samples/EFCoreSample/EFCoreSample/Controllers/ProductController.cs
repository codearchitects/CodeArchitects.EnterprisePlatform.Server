using EFCoreSample.Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFCoreSample.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("[controller]")]
public class ProductController : ControllerBase
{
  private readonly IProductRepository _productRepository;

  public ProductController(IProductRepository productRepository)
  {
    _productRepository = productRepository;
  }

  [HttpGet("withpricelessthan")]
  public async Task<ActionResult> GetProductsWithPriceLessThan(decimal price, CancellationToken cancellationToken)
  {
    var products = await _productRepository.GetProductsWithPriceLessThan(price, cancellationToken);
    return Ok(products);
  }

  [HttpGet("public")]
  public async Task<ActionResult> GetPublicProducts(CancellationToken cancellationToken)
  {
    var products = await _productRepository.GetPublicProducts(cancellationToken);
    return Ok(products);
  }
}
