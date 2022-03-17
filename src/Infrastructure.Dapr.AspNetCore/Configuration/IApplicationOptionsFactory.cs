using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using Microsoft.Extensions.FileProviders;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;

internal interface IApplicationOptionsFactory
{
  ApplicationOptions? FromFileProvider(IFileProvider provider);
}