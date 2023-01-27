using System.Linq.Expressions;

namespace CodeArchitects.Platform.Common.Expressions;

public class ExpressionEvaluatorTests
{
  [Fact]
  public void Evaluate_ShouldEvaluateConstantExpression()
  {
    // Arrange
    Expression<Func<int>> expr = () => 1;

    // Act
    int value = ExpressionEvaluator.Evaluate<int>(expr.Body);

    // Assert
    value.Should().Be(1);
  }

  [Theory]
  [InlineData(1)]
  public void Evaluate_ShouldEvaluateLocal(int arg)
  {
    // Arrange
    Expression<Func<int>> expr = () => arg;

    // Act
    int value = ExpressionEvaluator.Evaluate<int>(expr.Body);

    // Assert
    value.Should().Be(arg);
  }

  [Fact]
  public void Evaluate_ShouldEvaluateStaticFunctionCall()
  {
    // Arrange
    Expression<Func<int>> expr = TestClass.StaticFunctionCall();

    // Act
    int value = ExpressionEvaluator.Evaluate<int>(expr.Body);

    // Assert
    value.Should().Be(2);
  }

  [Fact]
  public void Evaluate_ShouldEvaluateInstanceFunctionCall()
  {
    // Arrange
    Expression<Func<int>> expr = new TestClass().InstanceFunctionCall();

    // Act
    int value = ExpressionEvaluator.Evaluate<int>(expr.Body);

    // Assert
    value.Should().Be(3);
  }

  [Theory]
  [InlineData(1)]
  public void Evaluate_ShouldEvaluateBinaryAdd(int arg)
  {
    // Arrange
    Expression<Func<int>> expr = () => arg + 2;

    // Act
    int value = ExpressionEvaluator.Evaluate<int>(expr.Body);

    // Assert
    value.Should().Be(3);
  }

  [Theory]
  [InlineData(1)]
  public void Evaluate_ShouldEvaluateNegate(int arg)
  {
    // Arrange
    Expression<Func<int>> expr = () => -arg;

    // Act
    int value = ExpressionEvaluator.Evaluate<int>(expr.Body);

    // Assert
    value.Should().Be(-1);
  }

  [Theory]
  [InlineData(1)]
  public void Evaluate_ShouldEvaluateFunc(int arg)
  {
    // Arrange
    Expression<Func<Func<int, int>>> expr = () => number => number + arg;

    // Act
    Func<int, int> value = ExpressionEvaluator.Evaluate<Func<int, int>>(expr.Body);

    // Assert
    Func<int, int> func = value.Should().BeAssignableTo<Func<int, int>>().Subject;
    func.Invoke(2).Should().Be(3);
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void Evaluate_ShouldEvaluateConditional(bool condition)
  {
    // Arrange
    Expression<Func<int>> expr = () => condition ? 1 : 2;

    // Act
    int value = ExpressionEvaluator.Evaluate<int>(expr.Body);

    // Assert
    value.Should().Be(condition ? 1 : 2);
  }

  [Fact]
  public void Evaluate_ShouldEvaluateNew()
  {
    // Arrange
    Expression<Func<Record>> expr = () => new Record(1, "2");

    // Act
    Record value = ExpressionEvaluator.Evaluate<Record>(expr.Body);

    // Assert
    value.Should().Be(new Record(1, "2"));
  }

  [Fact]
  public void Evaluate_ShouldEvaluateNewWithMemberAccessInitializers()
  {
    // Arrange
    Expression<Func<Record>> expr = () => new Record(1, "2") { Value1 = 3 };

    // Act
    Record value = ExpressionEvaluator.Evaluate<Record>(expr.Body);

    // Assert
    value.Should().Be(new Record(3, "2"));
  }

  [Fact]
  public void Evaluate_ShouldEvaluateNewWithMemberMemberInitializers()
  {
    // Arrange
    Expression<Func<ClassForMemberMemberInit>> expr = () => new ClassForMemberMemberInit { Member = { Value = 1 } };

    // Act
    ClassForMemberMemberInit value = ExpressionEvaluator.Evaluate<ClassForMemberMemberInit>(expr.Body);

    // Assert
    value.Member.Value.Should().Be(1);
  }

  [Fact]
  public void Evaluate_ShouldEvaluateNewWithMemberListInitializers()
  {
    // Arrange
    Expression<Func<ClassForMemberListInit>> expr = () => new ClassForMemberListInit { Member = { 1, 2, 3 } };

    // Act
    ClassForMemberListInit value = ExpressionEvaluator.Evaluate<ClassForMemberListInit>(expr.Body);

    // Assert
    value.Member.Should().BeEquivalentTo(new[] { 1, 2, 3 });
  }

  [Fact]
  public void Evaluate_ShouldEvaluateNewListWithInitializers()
  {
    // Arrange
    Expression<Func<List<int>>> expr = () => new List<int>() { 1, 2, 3 };

    // Act
    List<int> value = ExpressionEvaluator.Evaluate<List<int>>(expr.Body);

    // Assert
    value.Should().BeEquivalentTo(new[] { 1, 2, 3 });
  }

  [Fact]
  public void Evaluate_ShouldEvaluateNewArray()
  {
    // Arrange
    Expression<Func<int[]>> expr = () => new[] { 1, 2, 3 };

    // Act
    int[] value = ExpressionEvaluator.Evaluate<int[]>(expr.Body);

    // Assert
    value.Should().BeEquivalentTo(new[] { 1, 2, 3 });
  }

  [Fact]
  public void Evaluate_ShouldEvaluateConversion()
  {
    // Arrange
    object? local = "str";
    Expression<Func<string>> expr = () => (string)local;

    // Act
    string value = ExpressionEvaluator.Evaluate<string>(expr.Body);

    // Assert
    value.Should().Be("str");
  }

  [Fact]
  public void Evaluate_ShouldEvaluateParameter()
  {
    // Arrange
    Expression<Func<int, int>> expr = x => x;

    // Act
    int value = ExpressionEvaluator.Evaluate<int>(expr.Body, expr.Parameters, new object?[] { 12 });

    // Assert
    value.Should().Be(12);
  }

  private class TestClass
  {
    private readonly int _two = 2;

    public static Expression<Func<int>> StaticFunctionCall()
    {
      return () => Add1(1);
    }

    public Expression<Func<int>> InstanceFunctionCall()
    {
      return () => Add2(1);
    }

    private static int Add1(int arg) => arg + 1;

    private int Add2(int arg) => arg + _two;
  }

  private record struct Record(int Value1, string Value2);

  private class ClassForMemberMemberInit
  {
    public MemberClass Member { get; } = new();

    public class MemberClass
    {
      public int Value { get; set; }
    }
  }

  private class ClassForMemberListInit
  {
    public List<int> Member { get; } = new();
  }
}
