namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IAccessor
{
  object? Get(object target);
  void Set(object target, object? value);
}
