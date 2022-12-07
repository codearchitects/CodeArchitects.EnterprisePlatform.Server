namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class PrimaryKeyModel : IPrimaryKeyModel
{
  private readonly List<IPrimaryKeyColumnModel> _columns;

  public PrimaryKeyModel()
  {
    _columns = new();
  }

  protected abstract Getter<object?> GetValueCore { get; }

  protected abstract Setter<object?> SetValueCore { get; }

  public abstract bool IsComposite { get; }

  public abstract Type Type { get; }

  public IReadOnlyList<IPrimaryKeyColumnModel> Columns => _columns;

  public Getter<object?> GetValue => GetValueCore;

  public Setter<object?> SetValue => SetValueCore;

  public void AddColumn(IPrimaryKeyColumnModel column)
  {
    _columns.Add(column);
  }
}
