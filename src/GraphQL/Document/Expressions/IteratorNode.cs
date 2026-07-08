using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal abstract class IteratorNode : StreamingNode, IEnumerable, IEnumerator, IDisposable
{
  private Predicate<string>? _currentCondition;

  protected TEnumerator GetEnumerator<TEnumerator>(Predicate<string> condition, TEnumerator @this)
    where TEnumerator : IEnumerator
  {
    Debug.Assert(ReferenceEquals(@this, this)); // Subclasses should implement TEnumerator and pass 'this'

    _currentCondition = condition;
    return @this;
  }

  bool IEnumerator.MoveNext()
  {
    Debug.Assert(_currentCondition is not null); // If someone is calling MoveNext(), ensure they called GetEnumerator(string, TEnumerator) before
    return MoveNext(_currentCondition);
  }

  void IDisposable.Dispose()
  {
    _currentCondition = null; // Reset _currentCondition so the check inside MoveNext() is always valid
  }

  #region Not relevant

  [ExcludeFromCodeCoverage]
  object IEnumerator.Current => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  void IEnumerator.Reset() => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  public IEnumerator GetEnumerator() => throw new NotSupportedException();

  #endregion
}
