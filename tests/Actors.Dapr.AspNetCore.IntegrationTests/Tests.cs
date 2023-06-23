using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;

namespace CodeArchitects.Platform.Actors.Dapr.AspNetCore;

public sealed class Tests : IClassFixture<TestFixture>, IDisposable
{
  private readonly HttpClient _http;

  public Tests()
  {
    _http = new();
    _http.BaseAddress = new Uri("http://localhost:20100/");
  }

  [Theory]
  [InlineData(0, 2)]
  [InlineData(1, 1)]
  [InlineData(2, 2)]
  public async Task PolymorphicMethod_ShouldReturnCorrectResult(int implementation, int expectedImplementation)
  {
    // Arrange

    // Act
    PolymorphicResult? result = await _http.GetFromJsonAsync<PolymorphicResult>($"/virtual-actor/polymorphic-method?implementation={implementation}");

    // Assert
    CheckResult(result);
    result.Implementation.Should().Be(expectedImplementation);
  }

  [Fact]
  public async Task Schedule_ShouldScheduleActivity()
  {
    // Arrange
    const string output = "scheduling works";

    // Act
    IdResult? idResult = await _http.GetFromJsonAsync<IdResult>($"/virtual-actor/schedule?output={output}");
    CheckResult(idResult);

    await Task.Delay(6000);
    OutputResult? result = await _http.GetFromJsonAsync<OutputResult>($"/virtual-actor/{idResult.Id}/output");

    // Assert
    CheckResult(result);
    result.Output.Should().Be(output);
  }

  [Fact]
  public async Task BindingEnabler_ShouldExecuteBinding()
  {
    // Arrange

    // Act
    OutputResult? result = await _http.GetFromJsonAsync<OutputResult>($"/virtual-actor/binding-enabler");

    // Assert
    CheckResult(result);
    result.Output.Should().Be("binding");
  }

  [Fact]
  public async Task BindingDisabler_ShouldNotExecuteBinding()
  {
    // Arrange

    // Act
    OutputResult? result = await _http.GetFromJsonAsync<OutputResult>($"/virtual-actor/binding-disabler");

    // Assert
    CheckResult(result);
    result.Output.Should().Be("no binding");
  }

  [Fact]
  public async Task Messaging_ShouldReceiveMessage()
  {
    // Arrange
    const string output = "messaging works";

    // Act
    IdResult? idResult = await _http.GetFromJsonAsync<IdResult>($"/virtual-actor/messaging?output={output}");
    CheckResult(idResult);

    await Task.Delay(1000);
    OutputResult? result = await _http.GetFromJsonAsync<OutputResult>($"/virtual-actor/{idResult.Id}/output");

    // Assert
    CheckResult(result);
    result.Output.Should().Be(output);
  }

  [Fact]
  public async Task CreateAsync_ShouldSetCorrectState()
  {
    // Arrange
    const int state = 92;

    // Act
    IdResult? idResult = await _http.GetFromJsonAsync<IdResult>($"/test-actor/create?state={state}");
    CheckResult(idResult);

    await Task.Delay(1000);
    StateResult? result = await _http.GetFromJsonAsync<StateResult>($"/test-actor/{idResult.Id}/state");

    // Assert
    CheckResult(result);
    result.State.Should().Be(state);
  }

  public void Dispose()
  {
    _http.Dispose();
  }

  private static void CheckResult([NotNull] Result? result)
  {
    if (result is null)
    {
      Assert.Fail("Result was null.");
    }

    if (result.Error != null)
    {
      Assert.Fail(result.Error);
    }
  }

  private class PolymorphicResult : Result
  {
    public int Implementation { get; set; }
  }

  private class OutputResult : Result
  {
    public string? Output { get; set; }
  }

  private class IdResult : Result
  {
    public Guid Id { get; set; }
  }

  private class StateResult : Result
  {
    public int State { get; set; }
  }

  private class Result
  {
    public string? Error { get; set; }
  }
}
