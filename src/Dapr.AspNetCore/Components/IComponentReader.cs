using CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;
using Microsoft.Extensions.FileProviders;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

internal interface IComponentReader
{
  ComponentSchema FromFile(IFileInfo file);
}
