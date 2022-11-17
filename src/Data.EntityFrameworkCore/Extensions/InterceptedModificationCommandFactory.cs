using Microsoft.EntityFrameworkCore.Update;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class InterceptedModificationCommandFactory : IModificationCommandFactory
{
  public IModificationCommand CreateModificationCommand(in ModificationCommandParameters modificationCommandParameters)
  {
    return new InterceptedModificationCommand(modificationCommandParameters);
  }

  public INonTrackedModificationCommand CreateNonTrackedModificationCommand(in NonTrackedModificationCommandParameters modificationCommandParameters)
  {
    return new InterceptedModificationCommand(modificationCommandParameters);
  }
}
