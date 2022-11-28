namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class EntityEmitResult
{
  public EntityEmitResult(Type entityType, Type materializerType, Type comparerType)
  {
    EntityType = entityType;
    MaterializerType = materializerType;
    ComparerType = comparerType;
  }

  public Type EntityType { get; }
  public Type MaterializerType { get; }
  public Type ComparerType { get; }
}
