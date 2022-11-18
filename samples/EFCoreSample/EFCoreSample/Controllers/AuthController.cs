using EFCoreSample.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EFCoreSample.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
  private readonly IConfiguration _configuration;

  public AuthController(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  [HttpPost("login")]
  public ActionResult Login(LoginRequest request)
  {
    var handler = new JwtSecurityTokenHandler();

    string tenantId;
    if (request.UserName == "apple")
    {
      tenantId = "f2ae191d-8708-4ecb-9c53-f93b668a1744";
    }
    else if (request.UserName == "dell")
    {
      tenantId = "26b27fd0-6507-42e1-9674-b777136d2256";
    }
    else if (request.UserName == "admin")
    {
      tenantId = "8639353b-9cb7-4941-b581-f9e093e45959";
    }
    else
    {
      return Unauthorized();
    }

    List<Claim> claims = new()
    {
      new Claim(ClaimTypes.NameIdentifier, tenantId),
      new Claim(ClaimConstants.TenantId, tenantId)
    };

    JwtSecurityToken token = new JwtSecurityToken(
      claims: claims,
      expires: DateTime.Now.AddDays(1),
      signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SigningKey"]!)), SecurityAlgorithms.HmacSha256));

    return Ok(new LoginResponse
    {
      AccessToken = handler.WriteToken(token)
    });
  }
}
