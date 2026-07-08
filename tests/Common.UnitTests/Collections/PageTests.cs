namespace CodeArchitects.Platform.Common.Collections;

public class PageTests
{
  [Fact]
  public void CtorWithIReadOnlyCollection_ShouldThrowExactlyArgumentNullException_WhenElementsIsNull()
  {
    // Arrange
    IReadOnlyCollection<object> elements = null!;

    // Act
    Func<Page<object>> ctor = () => new Page<object>(elements, false);

    // Assert
    ctor.Should().ThrowExactly<ArgumentNullException>();
  }

  [Fact]
  public void CtorWithIEnumerable_ShouldThrowExactlyArgumentNullException_WhenElementsIsNull()
  {
    // Arrange
    IEnumerable<object> elements = null!;

    // Act
    Func<Page<object>> ctor = () => new Page<object>(elements, 0, false);

    // Assert
    ctor.Should().ThrowExactly<ArgumentNullException>();
  }

  [Fact]
  public void Ctors_ShouldBeEquivalent()
  {
    // Arrange
    IReadOnlyCollection<object> elements = Array.Empty<object>();

    // Act
    Page<object> sut1 = new(elements, false);
    Page<object> sut2 = new(elements, 0, false);

    // Assert
    sut1.Count.Should().Be(sut2.Count);
    sut1.HasNext.Should().Be(sut2.HasNext);
    ((object)sut1.Elements).Should().Be(sut2.Elements);
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void HasNext_ShouldBeCorrect(bool hasNext)
  {
    // Arrange
    IReadOnlyCollection<object> elements = Array.Empty<object>();

    // Act
    Page<object> sut = new(elements, hasNext);

    // Assert
    sut.HasNext.Should().Be(hasNext);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(10)]
  public void Count_ShouldBeCorrect(int count)
  {
    // Arrange
    IReadOnlyCollection<object> elements = new object[count];

    // Act
    Page<object> sut = new(elements, false);

    // Assert
    sut.Count.Should().Be(count);
  }

  [Fact]
  public void Elements_ShouldSequenceEqualSource()
  {
    // Arrange
    IReadOnlyCollection<int> elements = Enumerable.Range(0, 10).ToArray();

    // Act
    Page<int> sut = new(elements, false);

    // Assert
    sut.Elements.Should().Equal(elements);
  }

  [Fact]
  public void GetEnumerator_ShouldBeEquivalentToSourceEnumerator()
  {
    // Arrange
    IReadOnlyCollection<int> elements = Enumerable.Range(0, 10).ToArray();
    IEnumerator<int> sourceEnumerator = elements.GetEnumerator();

    // Act
    Page<int> sut = new(elements, false);
    IEnumerator<int> enumerator = sut.GetEnumerator();

    // Assert
    enumerator.Should().NotBe(sourceEnumerator);
    while (true)
    {
      bool moveNextSource = sourceEnumerator.MoveNext();
      bool moveNext = enumerator.MoveNext();

      if (moveNext)
      {
        moveNext.Should().Be(moveNextSource);
        enumerator.Current.Should().Be(sourceEnumerator.Current);
      }
      else
      {
        break;
      }
    }
  }
}
