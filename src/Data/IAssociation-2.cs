namespace CodeArchitects.Platform.Data
{
  public interface IAssociation<TKey1, TKey2> : IAssociation
  {
    new TKey1 Id1 { get; }
    new TKey1 Id2 { get; }
  }
}
