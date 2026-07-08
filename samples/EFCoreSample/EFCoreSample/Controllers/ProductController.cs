using EFCoreSample.Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFCoreSample.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("products")]
public class ProductController : ControllerBase
{
  private readonly IProductRepository _repository;

  public ProductController(IProductRepository repository)
  {
    _repository = repository;
  }

  [HttpGet("withpricelessthan")]
  public async Task<ActionResult> GetProductsWithPriceLessThan(decimal price, CancellationToken cancellationToken)
  {
    var products = await _repository.GetProductsWithPriceLessThan(price, cancellationToken);
    return Ok(products);
  }

  [HttpGet("public")]
  public async Task<ActionResult> GetPublicProducts(CancellationToken cancellationToken)
  {
    var products = await _repository.GetPublicProducts(cancellationToken);
    return Ok(products);
  }
}
