using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data.MongoDB.Navigation;

public class EmbeddedIncluderTests
{
  private static readonly IDataModel s_model = new DataModelBuilder()
    .AddEntity(typeof(IncludeOrder))
    .AddEntity(typeof(IncludeBuyer))
    .Build();

  private static Action Validate(IncludeAction<IncludeOrder> include) =>
    () => include(new EmbeddedIncluder<IncludeOrder>(s_model));

  #region Accepted: embedded navigations

  [Fact]
  public void Include_ShouldAcceptAnEmbeddedDocument() =>
    Validate(include => include.Include(order => order.Shipping)).Should().NotThrow();

  [Fact]
  public void Include_ShouldAcceptAnEmbeddedCollection() =>
    Validate(include => include.Include(order => order.Lines)).Should().NotThrow();

  [Fact]
  public void Include_ShouldAcceptAChainOfEmbeddedDocuments() =>
    Validate(include => include.Include(order => order.Shipping!.Address)).Should().NotThrow();

  [Fact]
  public void Include_ShouldAcceptAnAnonymousObjectOfEmbeddedNavigations() =>
    Validate(include => include.Include(order => new { order.Shipping, order.Lines })).Should().NotThrow();

  [Fact]
  public void Include_ShouldAcceptThenInclude_OnAnEmbeddedDocument() =>
    Validate(include => include.Include(order => order.Shipping, shipping => shipping.Include(s => s.Address)))
      .Should().NotThrow();

  [Fact]
  public void Include_ShouldAcceptThenInclude_OnAnEmbeddedCollection() =>
    Validate(include => include.Include(order => order.Lines, lines => lines.Include(line => line.Discount)))
      .Should().NotThrow();

  [Theory]
  [InlineData("Shipping")]
  [InlineData("Shipping.Address")]
  [InlineData("Lines.Discount")]
  public void Include_ShouldAcceptAPathOfEmbeddedNavigations(string path) =>
    Validate(include => include.Include(path)).Should().NotThrow();

  #endregion

  #region Rejected: not supported by the provider

  [Fact]
  public void Include_ShouldReject_AReferenceToAnotherCollection() =>
    Validate(include => include.Include(order => order.Buyer))
      .Should().Throw<NotSupportedException>().WithMessage("*IncludeOrder.Buyer*includeBuyers*");

  [Fact]
  public void Include_ShouldReject_ACollectionOfReferencesToAnotherCollection() =>
    Validate(include => include.Include(order => order.Watchers))
      .Should().Throw<NotSupportedException>().WithMessage("*IncludeOrder.Watchers*");

  [Fact]
  public void Include_ShouldReject_AReferenceAtTheEndOfAChain() =>
    Validate(include => include.Include(order => order.Shipping!.Courier))
      .Should().Throw<NotSupportedException>().WithMessage("*Shipping.Courier*");

  [Fact]
  public void Include_ShouldReject_AReferenceInsideAnAnonymousObject() =>
    Validate(include => include.Include(order => new { order.Lines, order.Buyer }))
      .Should().Throw<NotSupportedException>();

  [Fact]
  public void Include_ShouldReject_ThenIncludeReachingAnotherCollection() =>
    Validate(include => include.Include(order => order.Shipping, shipping => shipping.Include(s => s.Courier)))
      .Should().Throw<NotSupportedException>();

  [Fact]
  public void Include_ShouldReject_AReferenceInAPath() =>
    Validate(include => include.Include("Shipping.Courier"))
      .Should().Throw<NotSupportedException>();

  [Fact]
  public void Include_ShouldReject_AMemberThatIsNotPersisted() =>
    Validate(include => include.Include(order => order.Cached))
      .Should().Throw<NotSupportedException>().WithMessage("*not persisted*");

  #endregion

  #region Rejected: invalid include

  [Fact]
  public void Include_ShouldReject_AFilteredInclude() =>
    Validate(include => include.Include(order => order.Lines.Where(line => line.Discount != null)))
      .Should().Throw<InvalidOperationException>().WithMessage("*Filtered includes*");

  [Fact]
  public void Include_ShouldReject_AScalarValue() =>
    Validate(include => include.Include(order => order.Code))
      .Should().Throw<InvalidOperationException>().WithMessage("*scalar*");

  [Theory]
  [InlineData("Missing")]
  [InlineData("Shipping.Missing")]
  [InlineData("Shipping..Address")]
  public void Include_ShouldReject_AnInvalidPath(string path) =>
    Validate(include => include.Include(path)).Should().Throw<InvalidOperationException>();

  #endregion
}
