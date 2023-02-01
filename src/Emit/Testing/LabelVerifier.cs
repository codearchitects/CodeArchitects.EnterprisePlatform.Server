namespace CodeArchitects.Platform.Emit.Testing;

internal class LabelVerifier
{
  private readonly IReadOnlyList<FakeLabel> _labels;
  private int _index;

  public LabelVerifier(IReadOnlyList<FakeLabel> labels)
  {
    _labels = labels;
  }

  public void VerifyComplete()
  {
    if (_index != _labels.Count)
      throw new Exception($"Remaining {_labels.Count - _index} labels were not verified.");
  }

  public LabelVerifier Position(int position)
  {
    FakeLabel label = _labels[_index];

    if (label.Position != position)
      throw new Exception($"Expected label at index {_index} to have position '{position}', but found it at position '{label.Position}'.");

    _index++;
    return this;
  }
}
