using System.ComponentModel.DataAnnotations;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore.Configuration;

public class OutputBindingConfig
{
  [Required]
  public string Name { get; set; } = default!;

  public List<object> Arguments { get; set; } = new();
}
