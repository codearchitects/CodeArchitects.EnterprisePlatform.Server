namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class MaterializerTypeInfo
{
  public MaterializerTypeInfo(Type entityType, Type materializerType)
  {
    EntityType = entityType;
    MaterializerType = materializerType;
  }

  public Type EntityType { get; }
  public Type MaterializerType { get; }
}
