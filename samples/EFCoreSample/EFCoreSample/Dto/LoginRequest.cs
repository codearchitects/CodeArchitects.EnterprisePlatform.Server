using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Dto;

public class LoginRequest
{
  [Required]
  public string UserName { get; init; } = default!;

  [Required]
  public string Password { get; init; } = default!;
}
