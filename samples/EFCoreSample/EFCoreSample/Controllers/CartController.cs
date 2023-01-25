using AutoMapper;
using EFCoreSample.Domain.Model;
using EFCoreSample.Domain.Repositories;
using EFCoreSample.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EFCoreSample.Controllers;

[ApiController]
[Route("carts")]
public class CartController : ControllerBase
{
  private readonly IMapper _mapper;
  private readonly ICartRepository _repository;

  public CartController(IMapper mapper, ICartRepository repository)
  {
    _mapper = mapper;
    _repository = repository;
  }

  [HttpGet]
  public async Task<ActionResult> GetCart(Guid id)
  {
    Cart? cart = await _repository.FindAsync(id, _ => _
      .Include(cart => cart.Orders, _ => _
        .Include(order => order.Products)));
    return cart is null ? NotFound() : Ok(_mapper.Map<CartDto>(cart));
  }

  [HttpPost]
  public async Task<ActionResult> CreateCart(CartDto dto)
  {
    Cart cart = _mapper.Map<Cart>(dto);
    await _repository.InsertAsync(cart);
    return Ok();
  }

  [HttpPut]
  public async Task<ActionResult> UpdateCart(CartDto dto)
  {
    Cart cart = _mapper.Map<Cart>(dto);
    await _repository.UpdateAsync(cart);
    return Ok();
  }
}
