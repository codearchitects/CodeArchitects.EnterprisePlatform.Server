namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract class AssociationBuilder : BuilderBase
{
  public abstract Association Build(AssociationKind kind);
}
