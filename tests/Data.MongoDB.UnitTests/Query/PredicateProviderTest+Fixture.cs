using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model;
using System.Linq.Expressions;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

public partial class PredicateProviderTests
{
  class KeyPredicate : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      IPredicateTemplateFactory templateFactory;
      LambdaExpression? template;
      Mock<IPredicateTemplateCache> cacheMock;

      // With cached template

      template = PredicateTemplates.EntityWithIdPropertyKeyTemplate;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(Guid), out template))
        .Returns(true)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };

      // Without cached template

      template = null;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(x =>
        x.BuildPredicateTemplate<EntityWithIdProperty, Guid>(It.IsAny<IEntityModel>()) == PredicateTemplates.EntityWithIdPropertyKeyTemplate,
        MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.AddTemplate(typeof(Guid), PredicateTemplates.EntityWithIdPropertyKeyTemplate))
        .Verifiable();
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(Guid), out template))
        .Returns(false)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };
    }
  }

  class EntityPredicate : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      IPredicateTemplateFactory templateFactory;
      LambdaExpression? template;
      Mock<IPredicateTemplateCache> cacheMock;

      // With cached template

      template = PredicateTemplates.EntityWithIdPropertyEntityTemplate;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithIdProperty), out template))
        .Returns(true)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };

      // Without cached template

      template = null;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(x =>
        x.BuildPredicateTemplate<EntityWithIdProperty>(It.IsAny<IEntityModel>()) == PredicateTemplates.EntityWithIdPropertyEntityTemplate,
        MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.AddTemplate(typeof(EntityWithIdProperty), PredicateTemplates.EntityWithIdPropertyEntityTemplate))
        .Verifiable();
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithIdProperty), out template))
        .Returns(false)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };
    }
  }
}
