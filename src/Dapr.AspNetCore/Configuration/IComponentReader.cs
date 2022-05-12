using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using Microsoft.Extensions.FileProviders;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Configuration;

internal interface IComponentReader
{
  ComponentSchema FromFile(IFileInfo file);
}
