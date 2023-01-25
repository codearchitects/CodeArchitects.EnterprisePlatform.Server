using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;
using System.Linq.Expressions;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

public partial class PredicateProviderTests
{
  class EntityWithSimplePropertyKeyDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      IPredicateTemplateFactory templateFactory;
      LambdaExpression? template;
      Mock<IPredicateTemplateCache> cacheMock;

      // With cached template

      template = FindPredicateTemplates.SimplePropertyKeyTemplate;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithSimplePropertyKey), out template))
        .Returns(true)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };

      // Without cached template

      template = null;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(x =>
        x.CreateFindPredicateTemplate<EntityWithSimplePropertyKey, int>() == FindPredicateTemplates.SimplePropertyKeyTemplate,
        MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.AddTemplate(typeof(EntityWithSimplePropertyKey), FindPredicateTemplates.SimplePropertyKeyTemplate))
        .Verifiable();
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithSimplePropertyKey), out template))
        .Returns(false)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };
    }
  }

  class EntityWithSimpleFieldKeyDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      IPredicateTemplateFactory templateFactory;
      LambdaExpression? template;
      Mock<IPredicateTemplateCache> cacheMock;

      // With cached template

      template = FindPredicateTemplates.SimpleFieldKeyTemplate;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithSimpleFieldKey), out template))
        .Returns(true)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };

      // Without cached template

      template = null;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(x =>
        x.CreateFindPredicateTemplate<EntityWithSimpleFieldKey, int>() == FindPredicateTemplates.SimpleFieldKeyTemplate,
        MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.AddTemplate(typeof(EntityWithSimpleFieldKey), FindPredicateTemplates.SimpleFieldKeyTemplate))
        .Verifiable();
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithSimpleFieldKey), out template))
        .Returns(false)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };
    }
  }

  class EntityWithSimpleShadowKeyDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      IPredicateTemplateFactory templateFactory;
      LambdaExpression? template;
      Mock<IPredicateTemplateCache> cacheMock;

      // With cached template

      template = FindPredicateTemplates.SimpleShadowKeyTemplate;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithSimpleShadowKey), out template))
        .Returns(true)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };

      // Without cached template

      template = null;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(x =>
        x.CreateFindPredicateTemplate<EntityWithSimpleShadowKey, int>() == FindPredicateTemplates.SimpleShadowKeyTemplate,
        MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.AddTemplate(typeof(EntityWithSimpleShadowKey), FindPredicateTemplates.SimpleShadowKeyTemplate))
        .Verifiable();
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithSimpleShadowKey), out template))
        .Returns(false)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };
    }
  }

  class EntityWithCompositeKeyDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      IPredicateTemplateFactory templateFactory;
      LambdaExpression? template;
      Mock<IPredicateTemplateCache> cacheMock;

      // With cached template

      template = FindPredicateTemplates.CompositeKeyTemplate;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithCompositeKey), out template))
        .Returns(true)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };

      // Without cached template

      template = null;
      templateFactory = Mock.Of<IPredicateTemplateFactory>(x =>
        x.CreateFindPredicateTemplate<EntityWithCompositeKey, (int, string)>() == FindPredicateTemplates.CompositeKeyTemplate,
        MockBehavior.Strict);
      cacheMock = new(MockBehavior.Strict);
      cacheMock
        .Setup(x => x.AddTemplate(typeof(EntityWithCompositeKey), FindPredicateTemplates.CompositeKeyTemplate))
        .Verifiable();
      cacheMock
        .Setup(x => x.TryGetTemplate(typeof(EntityWithCompositeKey), out template))
        .Returns(false)
        .Verifiable();

      yield return new object?[] { templateFactory, cacheMock };
    }
  }
}
