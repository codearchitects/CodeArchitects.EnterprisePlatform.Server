namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IAccessor<T>
{
  T Get(object target);

  void Set(object target, T value);
}
