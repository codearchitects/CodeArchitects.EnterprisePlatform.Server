using Microsoft.Extensions.FileProviders;
using System.Collections;
using System.Collections.Generic;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration.Fakes;

internal class FakeDirectoryContents : IDirectoryContents
{
  private readonly IEnumerable<IFileInfo> _files;

  public FakeDirectoryContents(IEnumerable<IFileInfo> files)
  {
    _files = files;
  }

  public bool Exists => true;

  public IEnumerator<IFileInfo> GetEnumerator() => _files.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_files).GetEnumerator();
}
