using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IAccessiblePropertyModelBase : IPropertyModelBase
{
  new Getter<object?> Getter { get; }

  new Setter<object?> Setter { get; }
}
