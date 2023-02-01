namespace CodeArchitects.Platform.Emit.Testing;

internal class LocalVerifier
{
  private readonly IReadOnlyList<FakeLocalBuilder> _locals;
  private int _index;

  public LocalVerifier(IReadOnlyList<FakeLocalBuilder> locals)
  {
    _locals = locals;
  }

  public void VerifyComplete()
  {
    if (_index != _locals.Count)
      throw new Exception($"Remaining {_locals.Count - _index} locals were not verified.");
  }

  public LocalVerifier OfType(Type localType, bool pinned = false)
  {
    FakeLocalBuilder local = _locals[_index];

    if (local.LocalType != localType)
      throw new Exception($"Expected local at index '{_index}' to have type '{localType}', but found '{local.LocalType}'.");

    if (local.IsPinned && !pinned)
    {
      throw new Exception($"Expected local at index '{_index}' to be not pinned, but such local is.");
    }
    else if (!local.IsPinned && pinned)
    {
      throw new Exception($"Expected local at index '{_index}' to be pinned, but such local is not.");
    }

    _index++;
    return this;
  }
}
