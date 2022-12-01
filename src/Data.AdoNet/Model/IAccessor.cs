namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IAccessor
{
  object? Get(object target);

  void Set(object target, object? value);
}
