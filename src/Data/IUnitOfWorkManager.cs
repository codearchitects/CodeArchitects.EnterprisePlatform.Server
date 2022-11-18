namespace CodeArchitects.Platform.Data;

public interface IUnitOfWorkManager
{
  IUnitOfWork Begin(bool autoSave = false, CancellationToken cancellationToken = default);
}
